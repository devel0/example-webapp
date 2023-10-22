namespace ExampleWebApp.Backend.WebApi;

/// <summary>
/// <see cref="AuthController.RegisterUser(RegisterUserRequestDto)"/> api request data.
/// </summary>
public class RegisterUserRequestDto
{    
    
    /// <summary>
    /// User name.
    /// </summary>
    public required string UserName { get; set; }
    
    /// <summary>
    /// User email.
    /// </summary>    
    public required string Email { get; set; }
    
    /// <summary>
    /// User password.
    /// </summary>    
    public required string Password { get; set; }
    
}
