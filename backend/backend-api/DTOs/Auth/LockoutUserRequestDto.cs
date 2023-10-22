namespace ExampleWebApp.Backend.WebApi;

/// <summary>
/// <see cref="AuthController.LockoutUser(LockoutUserRequestDto)"/> api request data.
/// </summary>
public class LockoutUserRequestDto
{
    /// <summary>
    /// User name.
    /// </summary>    
    public required string UserName { get; set; }

    /// <summary>
    /// Lock out end (UTC).
    /// </summary>    
    public required DateTimeOffset LockoutEnd { get; set; }
}
