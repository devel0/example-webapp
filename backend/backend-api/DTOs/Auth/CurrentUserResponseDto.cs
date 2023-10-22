namespace ExampleWebApp.Backend.WebApi;

/// <summary>
/// <see cref="AuthController.CurrentUser"/> response api specific status.
/// </summary>
public enum CurrentUserStatus
{
    /// <summary>
    /// Authentication valid.
    /// </summary>        
    OK = STATUS_OK,

    /// <summary>
    /// Invalid authentication.
    /// </summary>        
    InvalidAuthentication = STATUS_InvalidAuthentication,

    /// <summary>
    /// Invalid argument.
    /// </summary>        
    InvalidArgument = STATUS_InvalidArgument,

    /// <summary>
    /// Invalid argument.
    /// </summary>        
    AccessTokenNotFound = STATUS_Custom + 0
}

/// <summary>
/// <see cref="AuthController.CurrentUser"/> api response data.
/// </summary>
public class CurrentUserResponseDto
{

    /// <summary>
    /// API specific status response.
    /// </summary>    
    public required CurrentUserStatus Status { get; set; }

    /// <summary>
    /// Login username.
    /// </summary>    
    public string UserName { get; set; } = "";

    /// <summary>
    /// Email address.
    /// </summary>    
    public string Email { get; set; } = "";

    /// <summary>
    /// List of roles associated to this user.
    /// </summary>    
    public List<string> Roles { get; set; } = new List<string>();

}
