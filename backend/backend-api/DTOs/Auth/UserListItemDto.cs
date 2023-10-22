namespace ExampleWebApp.Backend.WebApi;

/// <summary>
/// <see cref="AuthController.ListUsers"/> api response data.
/// </summary>
public class UserListItemResponseDto
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
    /// User roles.
    /// </summary>
    public required IList<string> Roles { get; set; }

    /// <summary>
    /// Access failed count.
    /// </summary>    
    public required int AccessFailedCount { get; set; }

    /// <summary>
    /// Email is confirmed.
    /// </summary>    
    public required bool EmailConfirmed { get; set; }

    /// <summary>
    /// Lockout end (UTC).
    /// </summary>
    public required DateTimeOffset? LockoutEnd { get; set; }

    /// <summary>
    /// User phone number.
    /// </summary>    
    public required string? PhoneNumber { get; set; }

    /// <summary>
    /// User phone number confirmed.
    /// </summary>    
    public required bool PhoneNumberConfirmed { get; set; }

    /// <summary>
    /// Two factor enabled.
    /// </summary>    
    public required bool TwoFactorEnabled { get; set; }

}
