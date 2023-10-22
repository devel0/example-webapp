using Microsoft.AspNetCore.Authentication;

namespace ExampleWebApp.Backend.WebApi;

public class AuthService : IAuthService
{
    readonly UserManager<ApplicationUser> userManager;
    readonly RoleManager<IdentityRole> roleManager;
    readonly SignInManager<ApplicationUser> signInManager;
    readonly IJWTService jwtService;
    readonly IHttpContextAccessor httpContextAccessor;
    readonly IHostEnvironment environment;
    readonly ILogger<AuthService> logger;
    readonly IConfiguration configuration;
    readonly IAuthenticationService authenticationService;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        SignInManager<ApplicationUser> signInManager,
        IJWTService jwtService,
        IHttpContextAccessor httpContextAccessor,
        IHostEnvironment environment,
        ILogger<AuthService> logger,
        IConfiguration configuration,
        IAuthenticationService authenticationService)
    {
        this.userManager = userManager;
        this.roleManager = roleManager;
        this.signInManager = signInManager;
        this.jwtService = jwtService;
        this.httpContextAccessor = httpContextAccessor;
        this.environment = environment;
        this.logger = logger;
        this.configuration = configuration;
        this.authenticationService = authenticationService;
    }

    public async Task<ICommonResponseDto<LoginResponseDto>> LoginAsync(LoginRequestDto loginRequestDto,
        CancellationToken cancellationToken = default)
    {
        ApplicationUser? user = null;

        if (loginRequestDto.UserName is not null)
            user = await userManager.FindByNameAsync(loginRequestDto.UserName);
        else
        {
            if (loginRequestDto.Email is null) return new CommonResponseDto<LoginResponseDto>(
                HttpStatusCode.BadRequest,
                new LoginResponseDto { Status = LoginStatus.UsernameOrEmailRequired });

            user = await userManager.FindByEmailAsync(loginRequestDto.Email);
        }

        if (user is null || !(await userManager.CheckPasswordAsync(user, loginRequestDto.Password)))
            return new CommonResponseDto<LoginResponseDto>(
                HttpStatusCode.Unauthorized,
                new LoginResponseDto { Status = LoginStatus.InvalidAuthentication });

        var username = user.UserName;
        var email = user.Email;
        var claims = userManager.GetJWTClaims(user);
        if (username is null || email is null)
            throw new InternalErrorException("username or email null");

        var accessToken = jwtService.GenerateAccessToken(username, email, claims);
        var refreshToken = jwtService.GetValidRefreshToken(username);

        var persist = false; // TODO: 
        await signInManager.SignInWithClaimsAsync(user, persist, claims);

        var opts = new CookieOptions();
        environment.SetCookieOptions(opts);

        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext is null)
            return new CommonResponseDto<LoginResponseDto>(
                HttpStatusCode.BadRequest,
                new LoginResponseDto { Status = LoginStatus.InvalidHttpContext });

        var userName = user.UserName!;

        httpContext.Response.Cookies.Append(WEB_CookieName_XAccessToken, accessToken, opts);
        httpContext.Response.Cookies.Append(WEB_CookieName_XUsername, userName, opts);
        httpContext.Response.Cookies.Append(WEB_CookieName_XRefreshToken, refreshToken, opts);

        return new CommonResponseDto<LoginResponseDto>(
            HttpStatusCode.OK,
            new LoginResponseDto
            {
                Status = LoginStatus.OK,
                UserName = userName,
                Email = user.Email!,
                Roles = claims.GetRoles()
            }
        );
    }

    public async Task<ICommonResponseDto<RegisterUserResponseDto>> RegisterUserAsync(RegisterUserRequestDto registerUserRequestDto,
        CancellationToken cancellationToken = default)
    {
        var user = new ApplicationUser
        {
            UserName = registerUserRequestDto.UserName,
            Email = registerUserRequestDto.Email
        };

        var createRes = await userManager.CreateAsync(user, registerUserRequestDto.Password);
        if (createRes.Succeeded)
        {
            return new CommonResponseDto<RegisterUserResponseDto>(
                HttpStatusCode.OK,
                new RegisterUserResponseDto
                {
                    Status = RegisterUserStatus.OK,
                    Errors = new List<IdentityError>()
                });
        }
        else
        {
            var status = RegisterUserStatus.IdentityError;

            return new CommonResponseDto<RegisterUserResponseDto>(
                HttpStatusCode.BadRequest,
                new RegisterUserResponseDto
                {
                    Status = status,
                    Errors = createRes.Errors.ToList()
                });
        }
    }

    public async Task<ICommonResponseDto> LockoutUserAsync(LockoutUserRequestDto lockoutUserRequestDto,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByNameAsync(lockoutUserRequestDto.UserName);
        if (user is not null)
        {
            if (!IsUnitTesting())
                logger.LogInformation($"User [{user}] locked out untile {lockoutUserRequestDto.LockoutEnd}");
            await userManager.SetLockoutEndDateAsync(user, lockoutUserRequestDto.LockoutEnd);
            return new CommonResponseBaseDto(HttpStatusCode.OK);
        }

        else
            return new CommonResponseBaseDto(HttpStatusCode.BadRequest);
    }

    public async Task<ICommonResponseDto<CurrentUserResponseDto>> CurrentUserAsync(CancellationToken cancellationToken = default)
    {
        if (httpContextAccessor.HttpContext is null)
            return new CommonResponseDto<CurrentUserResponseDto>(
                HttpStatusCode.BadRequest,
                new CurrentUserResponseDto { Status = CurrentUserStatus.InvalidArgument });

        var quser = httpContextAccessor.HttpContext.User;

        if (quser is not null)
        {
            var userName = quser.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = quser.FindFirstValue(ClaimTypes.Email);

            if (userName is null || email is null)
                return new CommonResponseDto<CurrentUserResponseDto>(
                    HttpStatusCode.BadRequest,
                    new CurrentUserResponseDto { Status = CurrentUserStatus.InvalidAuthentication });

            // TODO: modularize cookie management
            var accessToken = httpContextAccessor.HttpContext.Request.Cookies[WEB_CookieName_XAccessToken];            

            if (accessToken is null)
                return new CommonResponseDto<CurrentUserResponseDto>(
                    HttpStatusCode.BadRequest,
                    new CurrentUserResponseDto { Status = CurrentUserStatus.AccessTokenNotFound });

            return new CommonResponseDto<CurrentUserResponseDto>(
                HttpStatusCode.OK,
                new CurrentUserResponseDto
                {
                    Status = CurrentUserStatus.OK,
                    UserName = userName,
                    Email = email,
                    Roles = quser.Claims.GetRoles()
                });
        }

        else
            return new CommonResponseDto<CurrentUserResponseDto>(
                HttpStatusCode.BadRequest,
                new CurrentUserResponseDto { Status = CurrentUserStatus.InvalidAuthentication });
    }

    public async Task<ICommonResponseDto> LogoutAsync(CancellationToken cancellationToken = default)
    {
        await signInManager.SignOutAsync();

        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext is null)
            return new CommonResponseDto<LoginResponseDto>(
                HttpStatusCode.BadRequest,
                new LoginResponseDto { Status = LoginStatus.InvalidHttpContext });

        var opts = new CookieOptions();
        environment.SetCookieOptions(opts);

        httpContext.Response.Cookies.Append(WEB_CookieName_XAccessToken, "", opts);
        httpContext.Response.Cookies.Append(WEB_CookieName_XUsername, "", opts);
        httpContext.Response.Cookies.Append(WEB_CookieName_XRefreshToken, "", opts);

        return new CommonResponseBaseDto(HttpStatusCode.OK);
    }

    public async Task<ICommonResponseDto<List<UserListItemResponseDto>>> ListUsersAsync(CancellationToken cancellationToken = default)
    {
        var res = new List<UserListItemResponseDto>();

        var users = await userManager.Users.ToListAsync();

        foreach (var user in users)
        {
            var roles = await userManager.GetRolesAsync(user);

            if (user.UserName is null || user.Email is null)
            {
                logger.LogWarning($"Inconsistent user (username or email null) id:{user.Id}");
                continue;
            }

            res.Add(new UserListItemResponseDto
            {
                UserName = user.UserName,
                Email = user.Email,
                AccessFailedCount = user.AccessFailedCount,
                EmailConfirmed = user.EmailConfirmed,
                LockoutEnd = user.LockoutEnd,
                PhoneNumber = user.PhoneNumber,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                Roles = roles,
                TwoFactorEnabled = user.TwoFactorEnabled
            });
        }

        return new CommonResponseDto<List<UserListItemResponseDto>>(HttpStatusCode.OK, res);
    }

    async Task<List<string>> AllRolesAsync()
    {
        return (await roleManager.Roles.ToListAsync())
            .Where(w => w.Name != null)
            .Select(w => w.Name!)
            .ToList();
    }

    public async Task<ICommonResponseDto<List<string>>> ListRolesAsync(CancellationToken cancellationToken = default)
    {
        var allRoles = await AllRolesAsync();

        if (allRoles is null)
        {
            logger.LogError("There are no roles");
            return new CommonResponseDto<List<string>>(HttpStatusCode.InternalServerError, new List<string>());
        }

        return new CommonResponseDto<List<string>>(HttpStatusCode.OK, allRoles);
    }

    public async Task<ICommonResponseDto<SetUserRolesResponseDto>> SetUserRolesAsync(SetUserRolesRequestDto setUserRolesRequestDto,
        CancellationToken cancellationToken = default)
    {
        if (setUserRolesRequestDto.UserName == configuration.GetConfigVar<string>(CONFIG_KEY_SeedUsers_Admin_UserName))
        {
            logger.LogWarning($"Can't change admin roles");
            return new CommonResponseDto<SetUserRolesResponseDto>(HttpStatusCode.BadRequest, new SetUserRolesResponseDto
            {
                Status = SetUserRolesStatus.AdminRolesReadOnly
            });
        }

        var user = await userManager.FindByNameAsync(setUserRolesRequestDto.UserName);

        if (user is null)
        {
            logger.LogWarning($"Can't find user [{setUserRolesRequestDto.UserName}]");
            return new CommonResponseDto<SetUserRolesResponseDto>(HttpStatusCode.BadRequest, new SetUserRolesResponseDto
            {
                Status = SetUserRolesStatus.UserNotFound
            });
        }

        var allRoles = await AllRolesAsync();
        var userRoles = await userManager.GetRolesAsync(user);
        var userRolesToSet = setUserRolesRequestDto.Roles.Where(roleToSet => allRoles.Contains(roleToSet)).ToList();

        if (userRoles is not null)
        {
            var rolesToAdd = userRolesToSet.Where(roleToSet => !userRoles.Contains(roleToSet));
            var rolesToRemove = userRoles.Where(userRole => !userRolesToSet.Contains(userRole));

            await userManager.AddToRolesAsync(user, rolesToAdd);
            await userManager.RemoveFromRolesAsync(user, rolesToRemove);

            if (!IsUnitTesting())
                logger.LogInformation(
                    $"Added [{string.Join(',', rolesToAdd)}] roles ; " +
                    $"Removed [{string.Join(',', rolesToRemove)}] roles ; " +
                    $"username:{setUserRolesRequestDto.UserName}");

            return new CommonResponseDto<SetUserRolesResponseDto>(HttpStatusCode.OK, new SetUserRolesResponseDto
            {
                Status = SetUserRolesStatus.OK,
                RolesAdded = rolesToAdd.ToList(),
                RolesRemoved = rolesToRemove.ToList()
            });
        }

        else
        {
            logger.LogError($"There are no roles for user {setUserRolesRequestDto.UserName}");
            return new CommonResponseDto<SetUserRolesResponseDto>(HttpStatusCode.InternalServerError, new SetUserRolesResponseDto
            {
                Status = SetUserRolesStatus.InternalError
            });
        }
    }

}
