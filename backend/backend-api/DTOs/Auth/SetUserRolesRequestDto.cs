namespace ExampleWebApp.Backend.WebApi;

/// <summary>
/// <see cref="AuthController.SetUserRoles(SetUserRolesRequestDto)"/> api request data.
/// </summary>
public class SetUserRolesRequestDto
{

    /// <summary>
    /// User name.
    /// </summary>    
    public required string UserName { get; set; }
    
    /// <summary>
    /// Roles to set to the user.
    /// </summary>    
    public required IList<string> Roles { get; set; }    

}
