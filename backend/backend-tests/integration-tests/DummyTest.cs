namespace ExampleWebApp.Backend.Tests.Integration;

// public class DummyTest : IClassFixture<CustomWebApplicationFactory<Program>>
// {

//     private readonly CustomWebApplicationFactory<Program> factory;

//     FactoryHelper fh;

//     public DummyTest(
//         CustomWebApplicationFactory<Program> factory)
//     {
//         this.factory = factory;

//         fh = new FactoryHelper(factory);

//         fh.Configuration.SetJwtTimeout(
//             accessTokenDuration: TimeSpan.FromSeconds(30),
//             refreshTokenDuration: TimeSpan.FromSeconds(60),
//             clockSkew: TimeSpan.FromSeconds(0));
//     }

//     [Fact]
//     public async Task CheckAnonymous()
//     {
//         Assert.Equal(HttpStatusCode.OK, (await fh.Client.GetAsync(API_Dummy_Anonymous)).StatusCode);
//         Assert.Equal(HttpStatusCode.Unauthorized, (await fh.Client.GetAsync(API_Dummy_Authorized)).StatusCode);
//         Assert.Equal(HttpStatusCode.Unauthorized, (await fh.Client.GetAsync(API_Dummy_RequireUserOrAdvancedOrAdmin)).StatusCode);
//         Assert.Equal(HttpStatusCode.Unauthorized, (await fh.Client.GetAsync(API_Dummy_RequireUserAndAdvanced)).StatusCode);
//         Assert.Equal(HttpStatusCode.Unauthorized, (await fh.Client.GetAsync(API_Dummy_RequireAdvancedOrAdmin)).StatusCode);
//         Assert.Equal(HttpStatusCode.Unauthorized, (await fh.Client.GetAsync(API_Dummy_RequireAdmin)).StatusCode);
//     }

//     const string USER_UserName = "user";
//     const string USER_Email = "user@user.com";
//     const string USER_Password = "Pass123!";

//     async Task RegisterUser(string[] roles)
//     {
//         var adminLoginDto = await AuthTest.Post_Login_Helper_Async(fh, fh.Configuration.AdminLoginDto_onlyUsername());
//         fh.Client.SetAccessToken(adminLoginDto.AccessToken);

//         var registerUser = await fh.Client.PostAsync(API_Auth_RegisterUser,
//             JsonContent.Create(new RegisterUserRequestDto { UserName = USER_UserName, Email = USER_Email, Password = USER_Password }));
//         Assert.Equal(HttpStatusCode.OK, registerUser.StatusCode);

//         var setUserRoles = await fh.Client.PostAsync(API_Auth_SetUserRoles,
//             JsonContent.Create(new SetUserRolesRequestDto { UserName = USER_UserName, Roles = roles }));
//         Assert.Equal(HttpStatusCode.OK, setUserRoles.StatusCode);
//     }

//     [Fact]
//     public async Task CheckAuthorized()
//     {
//         await RegisterUser(new string[] { });

//         var userLoginDto = await AuthTest.Post_Login_Helper_Async(fh, new LoginRequestDto
//         {
//             UserName = USER_UserName,
//             Password = USER_Password
//         });
//         fh.Client.SetAccessToken(userLoginDto.AccessToken);

//         Assert.Equal(HttpStatusCode.OK, (await fh.Client.GetAsync(API_Dummy_Anonymous)).StatusCode);
//         Assert.Equal(HttpStatusCode.OK, (await fh.Client.GetAsync(API_Dummy_Authorized)).StatusCode);
//         Assert.Equal(HttpStatusCode.Forbidden, (await fh.Client.GetAsync(API_Dummy_RequireUserOrAdvancedOrAdmin)).StatusCode);
//         Assert.Equal(HttpStatusCode.Forbidden, (await fh.Client.GetAsync(API_Dummy_RequireUserAndAdvanced)).StatusCode);
//         Assert.Equal(HttpStatusCode.Forbidden, (await fh.Client.GetAsync(API_Dummy_RequireAdvancedOrAdmin)).StatusCode);
//         Assert.Equal(HttpStatusCode.Forbidden, (await fh.Client.GetAsync(API_Dummy_RequireAdmin)).StatusCode);
//     }

//     [Fact]
//     public async Task CheckRoles_User()
//     {
//         await RegisterUser(new string[] { ROLE_user });

//         var userLoginDto = await AuthTest.Post_Login_Helper_Async(fh, new LoginRequestDto
//         {
//             UserName = USER_UserName,
//             Password = USER_Password
//         });
//         fh.Client.SetAccessToken(userLoginDto.AccessToken);

//         Assert.Equal(HttpStatusCode.OK, (await fh.Client.GetAsync(API_Dummy_Anonymous)).StatusCode);
//         Assert.Equal(HttpStatusCode.OK, (await fh.Client.GetAsync(API_Dummy_Authorized)).StatusCode);
//         Assert.Equal(HttpStatusCode.OK, (await fh.Client.GetAsync(API_Dummy_RequireUserOrAdvancedOrAdmin)).StatusCode);
//         Assert.Equal(HttpStatusCode.Forbidden, (await fh.Client.GetAsync(API_Dummy_RequireUserAndAdvanced)).StatusCode);
//         Assert.Equal(HttpStatusCode.Forbidden, (await fh.Client.GetAsync(API_Dummy_RequireAdvancedOrAdmin)).StatusCode);
//         Assert.Equal(HttpStatusCode.Forbidden, (await fh.Client.GetAsync(API_Dummy_RequireAdmin)).StatusCode);
//     }

//     [Fact]
//     public async Task CheckRoles_UserAndAdvanced()
//     {
//         await RegisterUser(new string[] { ROLE_user, ROLE_advanced });

//         var userLoginDto = await AuthTest.Post_Login_Helper_Async(fh, new LoginRequestDto
//         {
//             UserName = USER_UserName,
//             Password = USER_Password
//         });
//         fh.Client.SetAccessToken(userLoginDto.AccessToken);

//         Assert.Equal(HttpStatusCode.OK, (await fh.Client.GetAsync(API_Dummy_Anonymous)).StatusCode);
//         Assert.Equal(HttpStatusCode.OK, (await fh.Client.GetAsync(API_Dummy_Authorized)).StatusCode);
//         Assert.Equal(HttpStatusCode.OK, (await fh.Client.GetAsync(API_Dummy_RequireUserOrAdvancedOrAdmin)).StatusCode);
//         Assert.Equal(HttpStatusCode.OK, (await fh.Client.GetAsync(API_Dummy_RequireUserAndAdvanced)).StatusCode);
//         Assert.Equal(HttpStatusCode.OK, (await fh.Client.GetAsync(API_Dummy_RequireAdvancedOrAdmin)).StatusCode);
//         Assert.Equal(HttpStatusCode.Forbidden, (await fh.Client.GetAsync(API_Dummy_RequireAdmin)).StatusCode);
//     }

//     [Fact]
//     public async Task CheckRoles_advanced()
//     {
//         await RegisterUser(new string[] { ROLE_advanced });

//         var userLoginDto = await AuthTest.Post_Login_Helper_Async(fh, new LoginRequestDto
//         {
//             UserName = USER_UserName,
//             Password = USER_Password
//         });
//         fh.Client.SetAccessToken(userLoginDto.AccessToken);

//         Assert.Equal(HttpStatusCode.OK, (await fh.Client.GetAsync(API_Dummy_Anonymous)).StatusCode);
//         Assert.Equal(HttpStatusCode.OK, (await fh.Client.GetAsync(API_Dummy_Authorized)).StatusCode);
//         Assert.Equal(HttpStatusCode.OK, (await fh.Client.GetAsync(API_Dummy_RequireUserOrAdvancedOrAdmin)).StatusCode);
//         Assert.Equal(HttpStatusCode.Forbidden, (await fh.Client.GetAsync(API_Dummy_RequireUserAndAdvanced)).StatusCode);
//         Assert.Equal(HttpStatusCode.OK, (await fh.Client.GetAsync(API_Dummy_RequireAdvancedOrAdmin)).StatusCode);
//         Assert.Equal(HttpStatusCode.Forbidden, (await fh.Client.GetAsync(API_Dummy_RequireAdmin)).StatusCode);
//     }

//     [Fact]
//     public async Task CheckRoles_admin()
//     {        
//         var userLoginDto = await AuthTest.Post_Login_Helper_Async(fh, fh.Configuration.AdminLoginDto_onlyUsername());
//         fh.Client.SetAccessToken(userLoginDto.AccessToken);

//         Assert.Equal(HttpStatusCode.OK, (await fh.Client.GetAsync(API_Dummy_Anonymous)).StatusCode);
//         Assert.Equal(HttpStatusCode.OK, (await fh.Client.GetAsync(API_Dummy_Authorized)).StatusCode);
//         Assert.Equal(HttpStatusCode.OK, (await fh.Client.GetAsync(API_Dummy_RequireUserOrAdvancedOrAdmin)).StatusCode);
//         Assert.Equal(HttpStatusCode.Forbidden, (await fh.Client.GetAsync(API_Dummy_RequireUserAndAdvanced)).StatusCode);
//         Assert.Equal(HttpStatusCode.OK, (await fh.Client.GetAsync(API_Dummy_RequireAdvancedOrAdmin)).StatusCode);
//         Assert.Equal(HttpStatusCode.OK, (await fh.Client.GetAsync(API_Dummy_RequireAdmin)).StatusCode);
//     }


// }