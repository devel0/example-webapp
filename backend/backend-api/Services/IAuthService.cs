namespace ExampleWebApp.Backend.WebApi;

/// <summary>
/// Authentication service.
/// </summary>
public interface IAuthService
{

    /// <summary>
    /// Login user by given username or email and auth password.
    /// </summary>
    /// <param name="loginRequestDto">User login info.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Login response and JWT if successfully logged in.</returns>
    Task<ICommonResponseDto<LoginResponseDto>> LoginAsync(
        LoginRequestDto loginRequestDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create user by given username, email, password.
    /// </summary>
    /// <param name="registerUserRequestDto">Username, email, password of user to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Register user response.</returns>
    Task<ICommonResponseDto<RegisterUserResponseDto>> RegisterUserAsync(
        RegisterUserRequestDto registerUserRequestDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieve current logged in user name, email, roles.
    /// </summary>    
    /// <param name="cancellationToken">Cancellation token.</param>    
    /// <returns>Logged in user info or null if not authenticated.</returns>                
    Task<ICommonResponseDto<CurrentUserResponseDto>> CurrentUserAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Immediate user lockout until given time or unlock if time is in the past ( UTC ).
    /// Note that this happens when access token expires.
    /// </summary>    
    /// <param name="lockoutUserRequestDto">User to lockout info..</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><see cref="ICommonResponseDto"/></returns>                
    Task<ICommonResponseDto> LockoutUserAsync(
        LockoutUserRequestDto lockoutUserRequestDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// Logout current user.
    /// </summary>    
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<ICommonResponseDto> LogoutAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// List all users.
    /// </summary>    
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of users</returns>
    Task<ICommonResponseDto<List<UserListItemResponseDto>>> ListUsersAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// List all roles.
    /// </summary>    
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of users roles</returns>
    Task<ICommonResponseDto<List<string>>> ListRolesAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Change user roles
    /// </summary>    
    /// <param name="setUserRolesRequestDto">Data with username and roles to set.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Set user response.</returns>
    Task<ICommonResponseDto<SetUserRolesResponseDto>> SetUserRolesAsync(
        SetUserRolesRequestDto setUserRolesRequestDto, CancellationToken cancellationToken = default);

}
