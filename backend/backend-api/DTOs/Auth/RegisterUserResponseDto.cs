namespace ExampleWebApp.Backend.WebApi;

/// <summary>
/// <see cref="AuthController.RegisterUser(RegisterUserRequestDto)"/> response api specific status.
/// </summary>
public enum RegisterUserStatus
{
    /// <summary>
    /// Registration ok.
    /// </summary>    
    OK = STATUS_OK,

    /// <summary>
    /// Identity error.
    /// </summary>        
    IdentityError = STATUS_IdentityError,
}

/// <summary>
/// <see cref="AuthController.RegisterUser(RegisterUserRequestDto)"/> api response data.
/// </summary>
public class RegisterUserResponseDto
{

    /// <summary>
    /// API specific status response.
    /// </summary>    
    public required RegisterUserStatus Status { get; set; }

    /// <summary>
    /// List of register errors if any.
    /// </summary>    
    public required List<IdentityError> Errors { get; set; }

}
