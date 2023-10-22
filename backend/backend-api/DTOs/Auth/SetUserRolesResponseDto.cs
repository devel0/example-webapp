namespace ExampleWebApp.Backend.WebApi;

/// <summary>
/// <see cref="AuthController.SetUserRoles(SetUserRolesRequestDto)"/> response api specific status.
/// </summary>
public enum SetUserRolesStatus
{
    /// <summary>
    /// Set user roles successful.
    /// </summary>        
    OK = STATUS_OK,

    /// <summary>
    /// Can't change admin role.
    /// </summary>    
    AdminRolesReadOnly = STATUS_AdminRolesReadOnly,

    /// <summary>
    /// User not found.
    /// </summary>    
    UserNotFound = STATUS_UserNotFound,

    /// <summary>
    /// Internal error.
    /// </summary>    
    InternalError = STATUS_InternalError
}

/// <summary>
/// <see cref="AuthController.SetUserRoles(SetUserRolesRequestDto)"/> api response data.
/// </summary>
public class SetUserRolesResponseDto
{

    /// <summary>
    /// API specific status response.
    /// </summary>    
    public required SetUserRolesStatus Status { get; set; }

    /// <summary>
    /// Roles added.
    /// </summary>    
    public List<string> RolesAdded { get; set; } = new List<string>();

    /// <summary>
    /// Roles removed.
    /// </summary>    
    public List<string> RolesRemoved { get; set; } = new List<string>();

}
