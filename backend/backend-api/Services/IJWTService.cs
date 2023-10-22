namespace ExampleWebApp.Backend.WebApi;

/// <summary>
/// JWT Service helper.
/// </summary>
public interface IJWTService
{

    /// <summary>
    /// Duration of generated access token.<br/>
    /// In order to be able block a user the access token duration should a small interval, from 30min to max 2hr.<br/>
    /// </summary>    
    TimeSpan AccessTokenLifetime { get; }

    /// <summary>
    /// Duration of refresh token could up to 90 days.<br/>    
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