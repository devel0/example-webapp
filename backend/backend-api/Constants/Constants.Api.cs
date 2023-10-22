namespace ExampleWebApp.Backend.WebApi;

public static partial class Constants
{

    // api STATUS

    public const int STATUS_OK = 0;
    public const int STATUS_InternalError = 1;
    public const int STATUS_InvalidArgument = 2;
    public const int STATUS_InvalidAuthentication = 3;
    public const int STATUS_Unknown = 4;
    public const int STATUS_IdentityError = 5;
    public const int STATUS_AdminRolesReadOnly = 6;
    public const int STATUS_UserNotFound = 7;
    public const int STATUS_InvalidHttpContext = 8;

    public const int STATUS_Custom = 100;

    // Swagger

    public const string SWAGGER_ENDPOINT_PATH = "/swagger/v3/swagger.json";
    public const string SWAGGER_UI_PATH = "/swagger";
    public const string SWAGGER_CSS_PATH = "misc/SwaggerDark.css";

    // API endpoints

    public const string API_Auth_CurrentUser = "/api/Auth/CurrentUser";    
    public const string API_Auth_Login = "api/Auth/Login";
    public const string API_Auth_RenewAccessToken = "api/Auth/RenewAccessToken";
    public const string API_Auth_Logout = "api/Auth/Logout";
    public const string API_Auth_RegisterUser = "api/Auth/RegisterUser";
    public const string API_Auth_ListUsers = "api/Auth/ListUsers";
    public const string API_Auth_ListRoles = "api/Auth/ListRoles";
    public const string API_Auth_SetUserRoles = "api/Auth/SetUserRoles";
    public const string API_Auth_LockoutUser = "api/Auth/LockoutUser";

    public const string API_Dummy_Anonymous = "api/Dummy/Anonymous";
    public const string API_Dummy_Authorized = "api/Dummy/Authorized";
    public const string API_Dummy_RequireUserOrAdvancedOrAdmin = "api/Dummy/RequireUserOrAdvancedOrAdmin";
    public const string API_Dummy_RequireUserAndAdvanced = "api/Dummy/RequireUserAndAdvanced";
    public const string API_Dummy_RequireAdvancedOrAdmin = "api/Dummy/RequireAdvancedOrAdmin";
    public const string API_Dummy_RequireAdmin = "api/Dummy/RequireAdmin";

}