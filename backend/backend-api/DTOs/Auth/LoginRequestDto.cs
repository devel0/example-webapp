namespace ExampleWebApp.Backend.WebApi;

/// <summary>
/// <see cref="AuthController.Login(LoginRequestDto)"/> api request data.
/// </summary>
public class LoginRequestDto
{
    /// <summary>
    /// Username. If null but <see cref="Email"/> was given login will be tried within email as user identifier, instead of the username.
    /// </summary>    
    public string? UserName { get; set; }

    /// <summary>
    /// Email. Can be null if non null <see cref="UserName"/> given.
    /// </summary>    
    public string? Email { get; set; }

    /// <summary>
    /// Password.
    /// </summary>    
    public required string Password { get; set; }
}
