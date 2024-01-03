namespace ExampleWebApp.Backend.Tests.Integration;

public class AuthTest : IClassFixture<CustomWebApplicationFactory<Program>>
{

    public class LoginDtoRes
    {
        public required LoginResponseDto LoginDto { get; set; }
        public required JwtCookies JwtCookies { get; set; }
    }

    readonly CustomWebApplicationFactory<Program> factory;

    FactoryHelper fh;

    ILogger<Program> logger;

    public AuthTest(CustomWebApplicationFactory<Program> factory)
    {
        this.factory = factory;

        fh = new FactoryHelper(factory);
        logger = fh.logger;

        fh.Configuration.SetJwtTimeout(
            accessTokenDuration: TimeSpan.FromSeconds(30),
            refreshTokenDuration: TimeSpan.FromSeconds(60),
            clockSkew: TimeSpan.FromSeconds(0));
    }

    internal static async Task<LoginDtoRes> Post_Login_Helper_Async(
        FactoryHelper fh,
        HttpClient httpClient,
        LoginRequestDto loginRequestDto,
        HttpStatusCode expectedHttpStatusCode = HttpStatusCode.OK,
        LoginStatus expectedLoginStatus = LoginStatus.OK)
    {
        var login = await httpClient.PostAsync(API_Auth_Login, JsonContent.Create(loginRequestDto));
        Assert.Equal(expectedHttpStatusCode, login.StatusCode);

        var cookies = login.Headers.GetJwtCookiesFromResponse();

        var loginDto = JsonConvert.DeserializeObject<LoginResponseDto>(
            await login.Content.ReadAsStringAsync());
        Assert.NotNull(loginDto);
        Assert.Equal(expectedLoginStatus, loginDto.Status);

        return new LoginDtoRes
        {
            JwtCookies = cookies,
            LoginDto = loginDto,
        };
    }

    async Task<LoginDtoRes> Post_LoginAsAdmin_OK_Async(HttpClient httpClient)
    {
        var loginDtoRes = await Post_Login_Helper_Async(fh, httpClient, fh.Configuration.AdminLoginDto_onlyUsername());
        var loginDto = loginDtoRes.LoginDto;

        var loginData_full = fh.Configuration.AdminLoginDto();
        Assert.Equal(loginData_full.UserName, loginDto.UserName);
        Assert.Equal(loginData_full.Email, loginDto.Email);

        var jwtCookies = loginDtoRes.JwtCookies;

        Assert.NotNull(jwtCookies.AccessToken);
        Assert.NotEmpty(jwtCookies.AccessToken);

        Assert.NotNull(jwtCookies.RefreshToken);
        Assert.NotEmpty(jwtCookies.RefreshToken);

        return loginDtoRes;
    }

    // [Fact(DisplayName="ddd")] as workaround
    // https://github.com/microsoft/vscode-dotnettools/issues/295

    [Fact(DisplayName = nameof(Post_Login_as_Admin_onlyUserName_OK_Async))]
    public async Task<LoginDtoRes> Post_Login_as_Admin_onlyUserName_OK_Async()
    {
        var adminClient = fh.NewClient();
        return await Post_Login_Helper_Async(fh, adminClient, fh.Configuration.AdminLoginDto_onlyUsername());
    }

    [Fact(DisplayName = nameof(Post_LoginAsAdmin_UserName_and_Email_OK_Async))]
    public async Task<LoginDtoRes> Post_LoginAsAdmin_UserName_and_Email_OK_Async()
    {
        var adminclient = fh.NewClient();
        return await Post_Login_Helper_Async(fh, adminclient, fh.Configuration.AdminLoginDto());
    }

    [Fact(DisplayName = nameof(Post_LoginAsAdmin_empty_UnAuthorized_Async))]
    public async Task<LoginDtoRes> Post_LoginAsAdmin_empty_UnAuthorized_Async()
    {
        var adminclient = fh.NewClient();
        return await Post_Login_Helper_Async(fh, adminclient, new LoginRequestDto()
        {
            Password = "",
        },
        expectedHttpStatusCode: HttpStatusCode.BadRequest,
        expectedLoginStatus: LoginStatus.UsernameOrEmailRequired);
    }

    [Fact(DisplayName = nameof(Post_LoginAsAdmin_wrongUsername_UnAuthorized_Async))]
    public async Task<LoginDtoRes> Post_LoginAsAdmin_wrongUsername_UnAuthorized_Async()
    {
        var adminClient = fh.NewClient();
        return await Post_Login_Helper_Async(fh, adminClient, new LoginRequestDto()
        {
            UserName = "wrong",
            Password = "",
        },
        expectedHttpStatusCode: HttpStatusCode.Unauthorized,
        expectedLoginStatus: LoginStatus.InvalidAuthentication);
    }

    [Fact(DisplayName = nameof(Get_CurrentUser_is_UnAuthorized_without_ValidAuth_Async))]
    public async Task Get_CurrentUser_is_UnAuthorized_without_ValidAuth_Async()
    {
        // without access token current user api fail    
        var userClient = fh.NewClient();
        var currentUser = await userClient.GetAsync(API_Auth_CurrentUser);
        Assert.Equal(HttpStatusCode.Unauthorized, currentUser.StatusCode);

        Assert.Empty(await currentUser.Content.ReadAsStringAsync());
    }

    [Fact(DisplayName = nameof(Get_CurrentUser_is_Authorized_with_ValidAuth_Async))]
    public async Task<CurrentUserResponseDto> Get_CurrentUser_is_Authorized_with_ValidAuth_Async()
    {
        var adminClient = fh.NewClient();
        var loginDtoRes = await Post_LoginAsAdmin_OK_Async(adminClient);
        adminClient.SetAccessToken(loginDtoRes.JwtCookies.AccessToken!);

        var currentUser = await adminClient.GetAsync(API_Auth_CurrentUser);
        Assert.Equal(HttpStatusCode.OK, currentUser.StatusCode);

        var currentUserDto = JsonConvert.DeserializeObject<CurrentUserResponseDto>(
            await currentUser.Content.ReadAsStringAsync());
        Assert.NotNull(currentUserDto);
        Assert.Equal(CurrentUserStatus.OK, currentUserDto.Status);

        return currentUserDto;
    }

    async Task Get_CurrentUser_is_http_OK_Async()
    {
        var userClient = fh.NewClient();
        var currentUser = await userClient.GetAsync(API_Auth_CurrentUser);
        Assert.Equal(HttpStatusCode.OK, currentUser.StatusCode);
    }

    async Task Get_CurrentUser_is_http_Unauthorized_Async()
    {
        var userClient = fh.NewClient();
        var currentUser = await userClient.GetAsync(API_Auth_CurrentUser);
        Assert.Equal(HttpStatusCode.Unauthorized, currentUser.StatusCode);
    }

    [Fact(DisplayName = nameof(Get_CurrentUser_has_Username_and_Email_filled_Async))]
    public async Task Get_CurrentUser_has_Username_and_Email_filled_Async()
    {
        var userLoginDto = await Get_CurrentUser_is_Authorized_with_ValidAuth_Async();

        var adminUserLoginDto = fh.Configuration.AdminLoginDto();

        Assert.Equal(adminUserLoginDto.UserName, userLoginDto.UserName);
        Assert.Equal(adminUserLoginDto.Email, userLoginDto.Email);
    }

    [Fact(DisplayName = nameof(AuthValid_Before_RefreshTokenExpire_Async))]
    public async Task AuthValid_Before_RefreshTokenExpire_Async()
    {
        var adminClient = fh.NewClient();

        var accessTokenLifetime = TimeSpan.FromSeconds(3);
        var refreshTokenLifetime = TimeSpan.FromSeconds(4);

        fh.Configuration.SetJwtTimeout(
            accessTokenDuration: accessTokenLifetime,
            refreshTokenDuration: refreshTokenLifetime,
            clockSkew: TimeSpan.Zero);

        var reqStart = DateTime.UtcNow;
        await Task.Delay(TimeSpan.FromSeconds(1));

        var loginDtoRes = await Post_LoginAsAdmin_OK_Async(adminClient);
        Assert.NotNull(loginDtoRes);
        var jwt = loginDtoRes.JwtCookies.GetJwtSecurityToken();

        Assert.True(jwt.IssuedAt >= reqStart);
        Assert.True(jwt.ValidFrom >= reqStart);
        Assert.Equal(jwt.ValidFrom + accessTokenLifetime, jwt.ValidTo);

        while (DateTime.UtcNow <= jwt.ValidTo)
            await Task.Delay(100);

        await Task.Delay(100);

        // now access token expired, but refresh token still valid
        // use of refresh token still valid, impliies rotate within new lifetime span from now        

        var currentUser = await adminClient.GetAsync(API_Auth_CurrentUser);
        Assert.Equal(HttpStatusCode.OK, currentUser.StatusCode);

        var dtRotated = DateTime.UtcNow;

        while (DateTime.UtcNow <= dtRotated.Add(refreshTokenLifetime))
            await Task.Delay(100);

        await Task.Delay(100);
        // refresh token expired

        currentUser = await adminClient.GetAsync(API_Auth_CurrentUser);
        Assert.Equal(HttpStatusCode.Unauthorized, currentUser.StatusCode);
    }

    [Fact(DisplayName = nameof(Token_Params))]
    public async Task Token_Params()
    {
        var DRIFT = TimeSpan.FromSeconds(1);

        var configTimeout = fh.Configuration.GetConfigurationJwtTimeout();
        var userReqStart = DateTimeOffset.UtcNow;

        var adminClient = fh.NewClient();
        var adminLoginDtoRes = await Post_LoginAsAdmin_OK_Async(adminClient);
        var jwt = adminLoginDtoRes.JwtCookies.GetJwtSecurityToken();

        Assert.True(jwt.IssuedAt + DRIFT >= userReqStart);
        Assert.True(jwt.ValidFrom + DRIFT >= userReqStart);
        Assert.Equal(jwt.ValidFrom + configTimeout.accessTokenDuration, jwt.ValidTo);
    }

    [Fact(DisplayName = nameof(User_LockedOut_Async))]
    public async Task User_LockedOut_Async()
    {
        var adminClient = fh.NewClient();

        var ADMIN_ACCESS_TOKEN_DURATION = TimeSpan.FromSeconds(10);
        var USER_ACCESS_TOKEN_DURATION = TimeSpan.FromSeconds(3);
        var REFRESH_TOKEN_DURATION = TimeSpan.FromSeconds(6);
        var LOCKOUT_DURATION = TimeSpan.FromSeconds(1);

        var reqStart = DateTime.UtcNow;

        fh.Configuration.SetJwtTimeout(
            accessTokenDuration: ADMIN_ACCESS_TOKEN_DURATION,
            refreshTokenDuration: REFRESH_TOKEN_DURATION,
            clockSkew: TimeSpan.Zero);

        var adminLoginDtoRes = await Post_LoginAsAdmin_OK_Async(adminClient);
        adminClient.SetXCookies(adminLoginDtoRes.JwtCookies);

        var registerUserDto = await adminClient.PostAsync(API_Auth_RegisterUser,
            JsonContent.Create(new RegisterUserRequestDto { UserName = "user", Email = "user@user.com", Password = "Pass123!" }));
        Assert.Equal(HttpStatusCode.OK, registerUserDto.StatusCode);

        fh.Configuration.SetJwtTimeout(
            accessTokenDuration: USER_ACCESS_TOKEN_DURATION,
            refreshTokenDuration: REFRESH_TOKEN_DURATION,
            clockSkew: TimeSpan.Zero);

        var userClient = fh.NewClient();
        var userLoginDtoRes = await Post_Login_Helper_Async(fh, userClient, new LoginRequestDto
        {
            UserName = "user",
            Password = "Pass123!"
        }, expectedHttpStatusCode: HttpStatusCode.OK, expectedLoginStatus: LoginStatus.OK);
        userClient.SetXCookies(userLoginDtoRes.JwtCookies);
        var userJwt = userLoginDtoRes.JwtCookies.GetJwtSecurityToken();

        var userRefreshTokenIssuedAt = DateTimeOffset.UtcNow;

        var currentUser = await userClient.GetAsync(API_Auth_CurrentUser);
        Assert.Equal(HttpStatusCode.OK, currentUser.StatusCode);

        var currentUserRes = JsonConvert.DeserializeObject<CurrentUserResponseDto>(
            await currentUser.Content.ReadAsStringAsync());
        Assert.NotNull(currentUserRes);
        Assert.NotNull(currentUserRes.UserName);
        Assert.Equal("user", currentUserRes.UserName);

        var lockoutReq = new LockoutUserRequestDto
        {
            UserName = "user",
            LockoutEnd = userJwt.ValidTo + LOCKOUT_DURATION
        };
        // var adminJwt = adminLoginDtoRes.JwtCookies.GetJwtSecurityToken();
        // var timeNow = DateTime.UtcNow - reqStart;
        var lockoutRes = await adminClient.PostAsync(API_Auth_LockoutUser, JsonContent.Create(lockoutReq));
        Assert.Equal(HttpStatusCode.OK, lockoutRes.StatusCode);

        var dtLockoutBegin = DateTimeOffset.UtcNow;
        userClient.SetXCookies(userLoginDtoRes.JwtCookies);
        var currentUserAfterLockoutTry1 = await userClient.GetAsync(API_Auth_CurrentUser);
        Assert.Equal(HttpStatusCode.OK, currentUserAfterLockoutTry1.StatusCode); // current user still valid even locked out because access token

        while (DateTimeOffset.UtcNow <= userJwt.ValidTo)
            await Task.Delay(100);

        await Task.Delay(100);

        // user access token now expired and user locked out

        var currentUserAfterLockoutTry2 = await userClient.GetAsync(API_Auth_CurrentUser);
        Assert.Equal(HttpStatusCode.Unauthorized, currentUserAfterLockoutTry2.StatusCode);

        // after user lockout finished user can now access again

        while (DateTimeOffset.UtcNow <= lockoutReq.LockoutEnd)
            await Task.Delay(100);

        await Task.Delay(100);

        var currentUserAfterLockoutTry3 = await userClient.GetAsync(API_Auth_CurrentUser);
        Assert.Equal(HttpStatusCode.OK, currentUserAfterLockoutTry3.StatusCode);
    }

    /// <summary>
    /// generate a token with same content of the given one but signed with a random key
    /// </summary>
    string GenerateFakeAccessToken(string accessTokenTemplate)
    {
        var template = DecodeToJwtSecurityToken(accessTokenTemplate);
        if (template is null) throw new InternalErrorException("couldn't decode jwt token");

        var key = RandomJwtEncryptionKey();

        var accessTokenLifetime = TimeSpan.FromSeconds(
            fh.Configuration.GetConfigVar<int>(CONFIG_KEY_JwtSettings_AccessTokenDurationSeconds));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(template.Claims),
            Expires = DateTime.UtcNow.Add(accessTokenLifetime),
            Issuer = template.Issuer,
            Audience = template.Audiences.First(),
            SigningCredentials = new SigningCredentials(key, JWT_SecurityAlghoritm)
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    [Fact(DisplayName = nameof(Post_RenewAccessToken_invalid_WithFake_valid_WithExpired_Async))]
    public async Task Post_RenewAccessToken_invalid_WithFake_valid_WithExpired_Async()
    {
        var adminClient = fh.NewClient();
        var REFRESH_TOKEN_DURATION = TimeSpan.FromSeconds(6);

        fh.Configuration.SetJwtTimeout(
            accessTokenDuration: TimeSpan.FromSeconds(3),
            refreshTokenDuration: REFRESH_TOKEN_DURATION,
            clockSkew: TimeSpan.Zero);

        var reqStart = DateTime.UtcNow;

        // 1. login as admin

        var loginDtoRes = await Post_LoginAsAdmin_OK_Async(adminClient);
        Assert.NotNull(loginDtoRes);
        var jwt = loginDtoRes.JwtCookies.GetJwtSecurityToken();

        // 2. wait until access token expire

        while (DateTime.UtcNow <= jwt.ValidTo)
            await Task.Delay(100);
        
        var expiredAccessToken = loginDtoRes.JwtCookies.AccessToken!;
        var refreshToken = loginDtoRes.JwtCookies.RefreshToken!;

        var fakeAccessToken = GenerateFakeAccessToken(expiredAccessToken);

        // 3. a new access token can't be retrieved with an invalid signature access token
        //    and a valid refresh token

        {
            adminClient = fh.NewClient();
            adminClient.SetAccessToken(fakeAccessToken);
            adminClient.SetRefreshToken(refreshToken);

            var res = await adminClient.GetAsync(API_Auth_CurrentUser);
            Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
        }

        // 4. a new access token can be retrieved with an expired signature valid access token
        //    and a valid refresh token

        string rotatedRefreshToken;
        {
            adminClient = fh.NewClient();
            adminClient.DefaultRequestHeaders.Clear();
            adminClient.SetAccessToken(expiredAccessToken);
            adminClient.SetRefreshToken(refreshToken);

            var res = await adminClient.GetAsync(API_Auth_CurrentUser);
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);

            var cookies = res.Headers.GetJwtCookiesFromResponse();

            rotatedRefreshToken = cookies.RefreshToken;
            ;
        }

        var oldRefreshToken = refreshToken;

        // 5. a new access token can't be retrieved with previous refresh token, because rotated

        while (DateTime.UtcNow <= reqStart + REFRESH_TOKEN_DURATION)
            await Task.Delay(100);

        await Task.Delay(1000);

        {
            adminClient = fh.NewClient();
            adminClient.DefaultRequestHeaders.Clear();
            adminClient.SetAccessToken(expiredAccessToken);
            adminClient.SetRefreshToken(oldRefreshToken);

            var res = await adminClient.GetAsync(API_Auth_CurrentUser);
            Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
        }

        // 6. a new access token can be retrieved with rotated refresh token

        {
            adminClient = fh.NewClient();
            adminClient.DefaultRequestHeaders.Clear();
            adminClient.SetAccessToken(expiredAccessToken);
            adminClient.SetRefreshToken(rotatedRefreshToken);

            var res = await adminClient.GetAsync(API_Auth_CurrentUser);
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        }
    }

    [Fact(DisplayName = nameof(Post_RegisterUser_HasNoRoles))]
    public async Task Post_RegisterUser_HasNoRoles()
    {
        var adminClient = fh.NewClient();
        var loginDtoRes = await Post_LoginAsAdmin_OK_Async(adminClient);
        adminClient.SetAccessToken(loginDtoRes.JwtCookies.AccessToken!);

        var registerUserDto = await adminClient.PostAsync(API_Auth_RegisterUser,
            JsonContent.Create(new RegisterUserRequestDto { UserName = "user", Email = "user@user.com", Password = "Pass123!" }));
        Assert.Equal(HttpStatusCode.OK, registerUserDto.StatusCode);

        var listUsers = await adminClient.GetAsync(API_Auth_ListUsers);
        Assert.Equal(HttpStatusCode.OK, listUsers.StatusCode);

        var users = JsonConvert.DeserializeObject<List<UserListItemResponseDto>>(
            await listUsers.Content.ReadAsStringAsync());
        Assert.NotNull(users);

        Assert.Equal(2, users.Count);
        var qAdmin = users.FirstOrDefault(w => w.UserName == fh.Configuration.GetConfigVar(CONFIG_KEY_SeedUsers_Admin_UserName));
        var qUser = users.FirstOrDefault(w => w.UserName == "user");

        Assert.NotNull(qAdmin);
        Assert.NotNull(qUser);
    }

    [Fact(DisplayName = nameof(Post_SetUserRoles_add_user_then_user_advanced))]
    public async Task Post_SetUserRoles_add_user_then_user_advanced()
    {
        var adminClient = fh.NewClient();
        var loginDtoRes = await Post_LoginAsAdmin_OK_Async(adminClient);
        adminClient.SetAccessToken(loginDtoRes.JwtCookies.AccessToken!);

        var registerUserDto = await adminClient.PostAsync(API_Auth_RegisterUser,
            JsonContent.Create(new RegisterUserRequestDto { UserName = "user", Email = "user@user.com", Password = "Pass123!" }));
        Assert.Equal(HttpStatusCode.OK, registerUserDto.StatusCode);

        // set "user" role
        {
            var setUserRolesResponse = await adminClient.PostAsync(API_Auth_SetUserRoles,
                JsonContent.Create(new SetUserRolesRequestDto { UserName = "user", Roles = new[] { ROLE_user } }));
            Assert.Equal(HttpStatusCode.OK, setUserRolesResponse.StatusCode);


            var listUsers = await adminClient.GetAsync(API_Auth_ListUsers);
            Assert.Equal(HttpStatusCode.OK, listUsers.StatusCode);

            var users = JsonConvert.DeserializeObject<List<UserListItemResponseDto>>(
                await listUsers.Content.ReadAsStringAsync());
            Assert.NotNull(users);

            Assert.Equal(2, users.Count);
            var qAdmin = users.FirstOrDefault(w => w.UserName == fh.Configuration.GetConfigVar(CONFIG_KEY_SeedUsers_Admin_UserName));
            var qUser = users.FirstOrDefault(w => w.UserName == "user");

            Assert.NotNull(qAdmin);
            Assert.NotNull(qUser);

            Assert.Equal(1, qUser.Roles.Count);
            Assert.Equal(ROLE_user, qUser.Roles[0]);
        }

        // set "user", "advanced" roles
        {
            var setUserRolesResponse = await adminClient.PostAsync(API_Auth_SetUserRoles,
                JsonContent.Create(new SetUserRolesRequestDto { UserName = "user", Roles = new[] { ROLE_user, ROLE_advanced } }));
            Assert.Equal(HttpStatusCode.OK, setUserRolesResponse.StatusCode);

            var listUsers = await adminClient.GetAsync(API_Auth_ListUsers);
            Assert.Equal(HttpStatusCode.OK, listUsers.StatusCode);

            var users = JsonConvert.DeserializeObject<List<UserListItemResponseDto>>(
                await listUsers.Content.ReadAsStringAsync());
            Assert.NotNull(users);

            Assert.Equal(2, users.Count);
            var qAdmin = users.FirstOrDefault(w => w.UserName == fh.Configuration.GetConfigVar(CONFIG_KEY_SeedUsers_Admin_UserName));
            var qUser = users.FirstOrDefault(w => w.UserName == "user");

            Assert.NotNull(qAdmin);
            Assert.NotNull(qUser);

            Assert.Equal(2, qUser.Roles.Count);
            Assert.Contains(ROLE_user, qUser.Roles);
            Assert.Contains(ROLE_advanced, qUser.Roles);
        }
    }

}