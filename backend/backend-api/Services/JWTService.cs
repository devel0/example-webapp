namespace ExampleWebApp.Backend.WebApi;

public class JWTService : IJWTService
{
    readonly ILogger<JWTService> logger;
    readonly IConfiguration configuration;
    readonly UserManager<ApplicationUser> userManager;
    readonly ApplicationDbContext dbContext;

    public JWTService(
        ILogger<JWTService> logger,
        IConfiguration configuration,
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext dbContext)
    {
        this.logger = logger;
        this.configuration = configuration;
        this.userManager = userManager;
        this.dbContext = dbContext;
    }

    public TimeSpan AccessTokenLifetime => TimeSpan.FromSeconds(
        configuration.GetConfigVar<int>(CONFIG_KEY_JwtSettings_AccessTokenDurationSeconds));

    public TimeSpan RefreshTokenLifetime => TimeSpan.FromSeconds(
        configuration.GetConfigVar<int>(CONFIG_KEY_JwtSettings_RefreshTokenDurationSeconds));

    public SymmetricSecurityKey JwtEncryptionKey => new SymmetricSecurityKey(
        Convert.FromBase64String(configuration.GetConfigVar(CONFIG_KEY_JwtSettings_Key)));

    public string Issuer => configuration.GetConfigVar(CONFIG_KEY_JwtSettings_Issuer);

    public string Audience => configuration.GetConfigVar(CONFIG_KEY_JwtSettings_Audience);

    public string GenerateAccessToken(string username, string email, IList<Claim> claims)
    {
        var key = JwtEncryptionKey;

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.Add(AccessTokenLifetime),
            Issuer = Issuer,
            Audience = Audience,
            SigningCredentials = new SigningCredentials(JwtEncryptionKey, JWT_SecurityAlghoritm)
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.CreateToken(tokenDescriptor);

        var accessToken = tokenHandler.WriteToken(token);

        // logger.LogDebug($"generated access token : {accessToken}");

        return accessToken;
    }

    public RenewAccessTokenNfo? RenewAccessToken(string accessToken, string refreshToken)
    {
        var principal = GetPrincipalFromExpiredToken(accessToken);
        if (principal is null) return null; // invalid access token        

        var claimsUsername = principal.GetUserInfoFromClaims().UserName;

        if (claimsUsername is null) return null; // no username in claims        

        if (!IsRefreshTokenStillValid(claimsUsername, refreshToken)) return null; // refresh token not found                

        var quser = dbContext.Users.FirstOrDefault(w => w.UserName == claimsUsername);

        if (quser is null || quser.LockoutEnd > DateTime.UtcNow) return null; // user not exists any more or lockouted

        var email = quser.Email;
        if (email is null) throw new InternalErrorException("email not set");

        var resToken = GenerateAccessToken(claimsUsername, email, userManager.GetJWTClaims(quser));
        var resRefreshToken = GetValidRefreshToken(claimsUsername);

        return new RenewAccessTokenNfo
        {
            AccessToken = resToken,
            RefreshToken = resRefreshToken,
            UserName = claimsUsername,
            Principal = principal
        };
    }

    public string GetValidRefreshToken(string userName)
    {
        var qrefreshToken = dbContext.UserRefreshTokens
            .Where(x => x.UserName == userName)
            .ToList();

        var existingValidRefreshTokens = new List<UserRefreshToken>();

        foreach (var rt in qrefreshToken)
        {
            if (rt.Expires < DateTimeOffset.UtcNow) // remove expired refresh token            
                dbContext.UserRefreshTokens.Remove(rt);

            else // reuse existing valid refresh token                            
                existingValidRefreshTokens.Add(rt);
        }

        if (existingValidRefreshTokens.Count > 1)
        {
            var rt = existingValidRefreshTokens.OrderByDescending(w => w.Issued).First();

            // reuse recent refresh token

            return rt.RefreshToken;
        }

        // generate new refresh token

        string refreshToken;

        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            refreshToken = Convert.ToBase64String(randomNumber);
        }

        var utcNow = DateTimeOffset.UtcNow;

        var newRt = new UserRefreshToken
        {
            Issued = utcNow,
            Expires = utcNow + RefreshTokenLifetime,
            RefreshToken = refreshToken,
            UserName = userName
        };

        dbContext.UserRefreshTokens.Add(newRt);

        dbContext.SaveChanges();

        return refreshToken;
    }

    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string accessToken)
    {
        ClaimsPrincipal? principal;

        try
        {
            var tokenValidationParameters = configuration.GetTokenVaildationParameters(validateLifetime: false);

            var tokenHandler = new JwtSecurityTokenHandler();
            principal = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out var securityToken);
            if (securityToken is not JwtSecurityToken jwt || jwt.Header.Alg != JWT_SecurityAlghoritm)
                return null;
        }
        catch
        {
            // Microsoft.IdentityModel.Tokens.SecurityTokenSignatureKeyNotFoundException
            return null;
        }

        return principal;
    }

    public bool IsRefreshTokenStillValid(string userName, string refreshToken)
    {
        var qrefresh = dbContext.UserRefreshTokens
            .FirstOrDefault(w => w.UserName == userName && w.RefreshToken == refreshToken);

        if (qrefresh is null) return false; // refresh token associated with given username not found        

        var utcNow = DateTimeOffset.UtcNow;

        if (utcNow >= qrefresh.Expires) return false; // refresh token expired

        return true;
    }
}