namespace ExampleWebApp.Backend.WebApi;

/// <summary>
/// <see cref="AuthController.Login(LoginRequestDto)"/> response api specific status.
/// </summary>
public enum LoginStatus
{

    /// <summary>
    /// Login vaild.
    /// </summary>
    OK = STATUS_OK,

    /// <summary>
    /// Missing username or email.
    /// </summary>
    UsernameOrEmailRequired = STATUS_InvalidArgument,

    /// <summary>
    /// Invalid authentication.
    /// </summary>    
    InvalidAuthentication = STATUS_InvalidAuthentication,

    /// <summary>
    /// Authentication http context.
    /// </summary>    
    InvalidHttpContext = STATUS_InvalidHttpContext
}

/// <summary>
/// <see cref="AuthController.Login(LoginRequestDto)"/> api response data.
/// </summary>
public class LoginResponseDto
{

    /// <summary>
    /// API specific status response.
    /// </summary>    
    public required LoginStatus Status { get; set; }

    /// <summary>
    /// Username.
    /// </summary>    
    public string UserName { get; set; } = "";

    /// <summary>
    /// Email.
    /// </summary>    
    public string Email { get; set; } = "";

    /// <summary>
    /// User roles.
    /// </summary>
    public List<string> Roles { get; set; } = new List<string>();

}
