namespace ExampleWebApp.Backend.WebApi;

/// <summary>
/// Authentication controller
/// </summary>
[ApiController]
[Authorize]
[Route("api/[controller]/[action]")]
public class AuthController : ControllerBase
{

    readonly ILogger<AuthController> logger;
    readonly IAuthService authService;
    readonly IHttpContextAccessor httpContextAccessor;
    readonly IHostEnvironment environment;

    public AuthController(
        ILogger<AuthController> logger,
        IAuthService authService,
        IHttpContextAccessor httpContextAccessor,
        IHostEnvironment environment)
    {
        this.logger = logger;
        this.authService = authService;
        this.httpContextAccessor = httpContextAccessor;
        this.environment = environment;

        if (!IsUnitTesting())
            logger.LogTrace("Auth controller initialized.");
    }

    /// <summary>
    /// Retrieve current logged in user name, email, roles.
    /// </summary>    
    [HttpGet]
    public async Task<ActionResult<CurrentUserResponseDto>> CurrentUser() =>
        this.CommonResponse(await authService.CurrentUserAsync());

    /// <summary>
    /// Create user by given username, email, password.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = ROLE_admin)]
    public async Task<ActionResult<RegisterUserResponseDto>> RegisterUser(
        [FromBody] RegisterUserRequestDto registerUserRequestDto) =>
        this.CommonResponse(await authService.RegisterUserAsync(registerUserRequestDto));

    /// <summary>
    /// Immediate user lockout until given time or unlock if time is in the past ( UTC ).
    /// Note that this happens when access token expires.
    /// </summary>    
    [HttpPost]
    [Authorize(Roles = ROLE_admin)]
    public async Task<ActionResult> LockoutUser(
        [FromBody] LockoutUserRequestDto lockoutUserRequestDto) =>
        this.CommonResponse(await authService.LockoutUserAsync(lockoutUserRequestDto));

    /// <summary>
    /// Login user by given username or email and auth password.
    /// </summary>
    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponseDto>> Login(
        [FromBody] LoginRequestDto loginRequestDto) =>
        this.CommonResponse(await authService.LoginAsync(loginRequestDto));

    /// <summary>
    /// Logout current user.
    /// </summary>    
    [HttpGet]
    public async Task<ActionResult> Logout() =>
        this.CommonResponse(await authService.LogoutAsync());

    /// <summary>
    /// List all users.
    /// </summary>    
    [HttpGet]
    [Authorize(Roles = ROLE_admin)]
    public async Task<ActionResult<List<UserListItemResponseDto>>> ListUsers() =>
        this.CommonResponse(await authService.ListUsersAsync());

    /// <summary>
    /// List all roles.
    /// </summary>    
    [HttpGet]
    [Authorize(Roles = ROLE_admin)]
    public async Task<ActionResult<List<string>>> ListRoles() =>
        this.CommonResponse(await authService.ListRolesAsync());

    /// <summary>
    /// Change user roles
    /// </summary>    
    [HttpPost]
    [Authorize(Roles = ROLE_admin)]
    public async Task<ActionResult<SetUserRolesResponseDto>> SetUserRoles(SetUserRolesRequestDto setUserRolesRequestDto) =>
        this.CommonResponse(await authService.SetUserRolesAsync(setUserRolesRequestDto));

}
