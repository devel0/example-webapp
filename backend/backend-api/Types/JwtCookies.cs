namespace ExampleWebApp.Backend.WebApi.Types;

/// <summary>
/// Jwt cookie data.
/// </summary>
public class JwtCookies
{

    /// <summary>
    /// Access token.
    /// </summary>    
    public string? AccessToken { get; set; }

    /// <summary>
    /// User name.
    /// </summary>    
    public string? UserName { get; set; }

    /// <summary>
    /// Refresh token.
    /// </summary>    
    public string? RefreshToken { get; set; }
    
}
