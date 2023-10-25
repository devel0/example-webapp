namespace ExampleWebApp.Backend.WebApi;

/// <summary>
/// JWT Service helper.
/// </summary>
public interface IJWTService
{

    /// <summary>
    /// Duration of generated access token.<br/>    
    /// Keep as short as possible. (default: 5min)
    /// Small duration on access token makes effective the activation of the lockout of the user
    /// or the invalidation of refresh tokens following a password reset (pwd reset not yet implemented).
    /// </summary>    
    TimeSpan AccessTokenLifetime { get; }

    /// <summary>
    /// Duration of refresh token.<br/>      
    /// Keep as short as possible. (default: 20min)<br/>
    /// When the access token expires and refresh token is still valid then a new refresh token is generated
    /// with a renewd refrsh token duration from that start time; this mean the session can continue
    /// without further authentication even if the refresh token have a small duration.<br/>
    /// Small duration on refresh token reduce risk of crfs attack because when refresh token expires
    /// the only way to authorize a new request is to authenticate again through login.<br/>    
    /// </summary>    
    TimeSpan RefreshTokenLifetime { get; }

    /// <summary>
    /// JWT encryption key.
    /// </summary>    
    SymmetricSecurityKey JwtEncryptionKey { get; }

    /// <summary>
    /// Issuer identifier.
    /// </summary>    
    string Issuer { get; }

    /// <summary>
    /// Application identifier.
    /// </summary>    
    string Audience { get; }

    /// <summary>
    /// Generate access token for given username, email, claims with duration from now plus <see cref="AccessTokenLifetime"/>.
    /// </summary>    
    string GenerateAccessToken(string username, string email, IList<Claim> claims);

    /// <summary>
    /// Renew given access token when it contains a principal not locked out 
    /// within related given refresh token not yet expired.
    /// </summary>
    /// <param name="accessToken">Access token ( even if its expired ).</param>
    /// <param name="refreshToken">Valid Refresh Token.</param>    
    /// <returns>Null if refresh token isn't valid and a new login required.</returns>
    RenewAccessTokenNfo? RenewAccessToken(string accessToken, string refreshToken);

    /// <summary>
    /// Purge expired refresh tokens and reuse latest valid refresh token if there are more than one,
    /// else creates a new rotate refresh token and return that.
    /// </summary>    
    string GetValidRefreshToken(string userName);

    /// <summary>
    /// Retrieve principal from accessToken even its expired.
    /// </summary>
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string accessToken);

    /// <summary>
    /// States if given refreshToken is still valid ( exists and not yet expired ).
    /// </summary>    
    bool IsRefreshTokenStillValid(string userName, string refreshToken);

}