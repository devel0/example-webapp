namespace ExampleWebApp.Backend.WebApi;

public static partial class Extensions
{

    /// <summary>
    /// Retrieve username and email from given claims principal.
    /// </summary>
    public static UserClaimsNfo GetUserInfoFromClaims(this ClaimsPrincipal claimsPrincipal)
    {
        string? userName = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
        string? email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);

        return new UserClaimsNfo
        {
            UserName = userName,
            Email = email
        };
    }

    /// <summary>
    /// Retrieve list of user claims from the given application user.
    /// </summary>
    public static List<Claim> GetJWTClaims(this UserManager<ApplicationUser> userManager, ApplicationUser user)
    {
        if (user.UserName is null || user.Email is null)
            throw new ArgumentException("username or email null");

        var userRoles = userManager.GetRolesAsync(user).GetAwaiter().GetResult();

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.UserName),
            new(JwtRegisteredClaimNames.Email, user.Email)
        };

        foreach (var userRole in userRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, userRole));
        }

        return claims;
    }

    /// <summary>
    /// Adds missing roles to database from <see cref="ROLES_ALL"/> array source.
    /// </summary>    
    public static async Task UpgradeRolesAsync(this WebApplication webApplication)
    {
        using var scope = webApplication.Services.CreateScope();
        var rolemgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        var currentRoles = rolemgr.Roles.Where(w => w.Name != null).Select(w => w.Name!).ToList();

        var rolesToAdd = ROLES_ALL.Where(role => !currentRoles.Contains(role)).ToList();

        if (rolesToAdd.Count > 0)
        {
            if (!IsUnitTesting())
                webApplication.Logger.LogInformation($"Adding [{string.Join(',', rolesToAdd)}] roles");

            foreach (var roleToAdd in rolesToAdd)
            {
                await rolemgr.CreateAsync(new IdentityRole(roleToAdd));
            }
        }
    }

    /// <summary>
    /// Customize cookie name and HttpOnly, SameSite options.
    /// </summary>
    public static void SetupApplicationCookie(this WebApplicationBuilder builder)
    {
        builder.Services.ConfigureApplicationCookie(configure =>
        {
            configure.Cookie.Name = WEB_ApplicationCookieName;

            // configure.Events.OnRedirectToLogin = context =>
            // {
            //     ;
            //     return Task.CompletedTask;
            // };                        

            builder.Environment.SetCookieOptions(configure.Cookie);
        });
    }

    /// <summary>
    /// Add Identity provider with custom <see cref="ApplicationUser"/> user and system <see cref="IdentityRole"/> role management.
    /// Add <see cref="ApplicationDbContext"/> ef store for the identities.
    /// Add default token providers.
    /// </summary>        
    public static void SetupIdentityProvider(this IServiceCollection serviceCollection) => serviceCollection
        .AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

    /// <summary>
    /// Retrieve user roles from given list of claims.
    /// </summary>    
    public static List<string> GetRoles(this IEnumerable<Claim> claims) =>
        claims.Where(r => r.Type == ClaimTypes.Role).Select(w => w.Value).ToList();

    /// <summary>
    /// Get JWT token validation parameters from given options and current configuration.
    /// <seealso cref="CONFIG_KEY_JwtSettings_Issuer"/>
    /// <seealso cref="CONFIG_KEY_JwtSettings_Audience"/>
    /// <seealso cref="CONFIG_KEY_JwtSettings_Key"/>
    /// <seealso cref="CONFIG_KEY_JwtSettings_ClockSkewSeconds"/>
    /// </summary>    
    /// <param name="validateIssuer">Will validate issuer (default: true).</param>
    /// <param name="validateAudience">Will validate audience (default: true).</param>
    /// <param name="validateLifetime">Will validate access token lifetime (default: true).</param>
    /// <param name="validateIssuerSigningKey">Will validate issuer signing key (default: true).</param>
    /// <returns></returns>
    public static TokenValidationParameters GetTokenVaildationParameters(this IConfiguration configuration,
        bool validateIssuer = true,
        bool validateAudience = true,
        bool validateLifetime = true,
        bool validateIssuerSigningKey = true) =>
        new TokenValidationParameters
        {
            ValidateIssuer = validateIssuer,
            ValidateAudience = validateAudience,
            ValidateLifetime = validateLifetime,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration.GetConfigVar(CONFIG_KEY_JwtSettings_Issuer),
            ValidAudience = configuration.GetConfigVar(CONFIG_KEY_JwtSettings_Audience),
            IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(configuration.GetConfigVar(CONFIG_KEY_JwtSettings_Key))),
            ClockSkew = TimeSpan.FromSeconds(configuration.GetConfigVar<int>(CONFIG_KEY_JwtSettings_ClockSkewSeconds)),
        };

    static object lckRefreshToken = new object();

    /// <summary>
    /// Add authentication with JWT scheme.
    /// Add JWT bearer authentication with handling of failed authentication to resume within refresh token;
    /// it also extract jwt from X-Access-Token cookie.
    /// </summary>    
    public static void SetupJWTAuthentication(this WebApplicationBuilder builder) => builder.Services
        .AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = builder.Configuration.GetTokenVaildationParameters();

            if (options.Events == null) options.Events = new JwtBearerEvents();

            options.Events.OnAuthenticationFailed = async context =>
            {
                if (context.Exception.GetType() == typeof(SecurityTokenExpiredException) &&
                    context.HttpContext is not null)
                {
                    // access token expired

                    var jwtService = context.HttpContext.RequestServices.GetService<IJWTService>();
                    var userManager = context.HttpContext.RequestServices.GetService<UserManager<ApplicationUser>>();
                    var hostEnvironment = context.HttpContext.RequestServices.GetService<IHostEnvironment>();
                    var logger = context.HttpContext.RequestServices.GetService<ILogger<AuthController>>();

                    var accessToken = context.HttpContext.Request.Cookies[WEB_CookieName_XAccessToken];
                    var refreshToken = context.HttpContext.Request.Cookies[WEB_CookieName_XRefreshToken];

                    if (jwtService is not null && hostEnvironment is not null &&
                        userManager is not null && logger is not null &&
                        !string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(refreshToken))
                    {
                        var principal = jwtService.GetPrincipalFromExpiredToken(accessToken);
                        var username = principal?.GetUserInfoFromClaims().UserName;

                        if (username is not null &&
                            jwtService.IsRefreshTokenStillValid(username, refreshToken))
                        {
                            var user = await userManager.FindByNameAsync(username);
                            var dtNow = DateTime.UtcNow;
                            var userIsLockedOut = user is not null && await userManager.IsLockedOutAsync(user);

                            if (user is not null && !userIsLockedOut)
                            {
                                lock (lckRefreshToken)
                                {
                                    var res = jwtService.RenewAccessToken(accessToken, refreshToken);

                                    if (res is not null)
                                    {
                                        if (!IsUnitTesting())
                                            logger.LogTrace("refreshing token");

                                        var opts = new CookieOptions();

                                        hostEnvironment.SetCookieOptions(builder.Configuration, opts, setExpiresAsRefreshToken: true);
                                        context.HttpContext.Response.Cookies.Append(WEB_CookieName_XAccessToken, res.AccessToken, opts);
                                        context.HttpContext.Response.Cookies.Append(WEB_CookieName_XUsername, res.UserName, opts);
                                        context.HttpContext.Response.Cookies.Append(WEB_CookieName_XRefreshToken, res.RefreshToken, opts);

                                        context.Principal = res.Principal;
                                        context.Success();
                                    }
                                }
                            }
                        }
                    }
                }
            };

            options.Events.OnMessageReceived = context =>
            {
                if (context.Request.Cookies.ContainsKey(WEB_CookieName_XAccessToken))
                    context.Token = context.Request.Cookies[WEB_CookieName_XAccessToken];

                return Task.CompletedTask;
            };

        });

    /// <summary>
    /// Configure given CookieBuilder to set HttpOnly, Secure and SameSite options on created cookies.
    /// <seealso cref="COOKIE_OPTION_SECURE"/>
    /// <seealso cref="COOKIE_OPTION_HTTPONLY"/>
    /// </summary>
    public static void SetCookieOptions(this IHostEnvironment environment, CookieBuilder cookieBuilder)
    {
        if (environment.IsDevelopment())
        {
            cookieBuilder.SecurePolicy = CookieSecurePolicy.None;
            cookieBuilder.HttpOnly = true;
            cookieBuilder.SameSite = SameSiteMode.Lax;
        }
        else
        {
            cookieBuilder.SecurePolicy = CookieSecurePolicy.Always;
            cookieBuilder.HttpOnly = true;
            cookieBuilder.SameSite = SameSiteMode.Lax;
        }


    }

    /// <summary>
    /// Configure given CookieOptions to set HttpOnly, Secure and SameSite options on created cookies.
    /// <seealso cref="COOKIE_OPTION_SECURE"/>
    /// <seealso cref="COOKIE_OPTION_HTTPONLY"/>
    /// </summary>
    public static void SetCookieOptions(this IHostEnvironment environment, IConfiguration configuration,
         CookieOptions cookieOptions, bool setExpiresAsRefreshToken = false)
    {
        if (environment.IsDevelopment())
        {
            cookieOptions.Secure = false;
            cookieOptions.HttpOnly = true;
            cookieOptions.SameSite = SameSiteMode.Lax;
        }
        else
        {
            cookieOptions.Secure = true;
            cookieOptions.HttpOnly = true;
            cookieOptions.SameSite = SameSiteMode.Lax;
        }

        if (setExpiresAsRefreshToken)
        {
            var cookieDuration = TimeSpan.FromSeconds(
                configuration.GetConfigVar<int>(CONFIG_KEY_JwtSettings_RefreshTokenDurationSeconds));

            cookieOptions.Expires = DateTimeOffset.Now.Add(cookieDuration);
        }
    }

    /// <summary>
    /// Extracts X data from Set-Cookie cookie in given response headers.
    /// </summary>
    public static JwtCookies GetJwtCookiesFromResponse(this HttpResponseHeaders headers)
    {
        string? accessToken = null;
        string? userName = null;
        string? refreshToken = null;

        foreach (var hdr in headers)
        {
            switch (hdr.Key)
            {
                case WEB_HeadersCollection_SetCookie:
                    {
                        var cookies = SetCookieHeaderValue.ParseList(hdr.Value.ToList());

                        foreach (var cookie in cookies)
                        {
                            switch (cookie.Name.Value)
                            {
                                case WEB_CookieName_XAccessToken: accessToken = cookie.Value.Value; break;
                                case WEB_CookieName_XUsername: userName = cookie.Value.Value; break;
                                case WEB_CookieName_XRefreshToken: refreshToken = cookie.Value.Value; break;
                            }
                        }
                    }
                    break;
            }
        }

        return new JwtCookies
        {
            AccessToken = accessToken,
            UserName = userName,
            RefreshToken = refreshToken
        };
    }

    /// <summary>
    /// Extract X data from cookies in given request headers dictionary.
    /// </summary>
    public static JwtCookies GetJwtCookiesFromRequest(this IHeaderDictionary headers)
    {
        string? accessToken = null;
        string? userName = null;
        string? refreshToken = null;

        foreach (var cookie in headers.Cookie)
        {
            if (cookie is null) continue;

            var cookies = cookie.Split(';');

            foreach (var cookieKeyValue in cookies)
            {
                var ss = cookieKeyValue.Split('=');

                if (ss.Length == 2)
                {
                    var cookieKey = ss[0].Trim();
                    var cookieVal = ss[1].Trim();

                    switch (cookieKey)
                    {
                        case WEB_CookieName_XAccessToken: accessToken = HttpUtility.UrlDecode(cookieVal); break;
                        case WEB_CookieName_XUsername: userName = HttpUtility.UrlDecode(cookieVal); break;
                        case WEB_CookieName_XRefreshToken:
                            {
                                refreshToken = HttpUtility.UrlDecode(cookieVal);
                            }
                            break;
                    }
                }

            }
        }

        return new JwtCookies
        {
            AccessToken = accessToken,
            UserName = userName,
            RefreshToken = refreshToken
        };
    }

}
