namespace ExampleWebApp.Backend.WebApi;

public static partial class Constants
{

    public const bool COOKIE_OPTION_SECURE = true;
    public const bool COOKIE_OPTION_HTTPONLY = true;

    public const string ROLE_admin = "admin";
    public const string ROLE_user = "user";
    public const string ROLE_advanced = "advanced";

    /// <summary>
    /// user roles
    /// </summary>    
    public static readonly string[] ROLES_ALL = new[]
    {
        ROLE_admin,
        ROLE_user,
        ROLE_advanced
    };

}