namespace ExampleWebApp.Backend.WebApi;

public static partial class Constants
{

    public const string CONFIG_KEY_DbLoggingEnabled = "DbLoggingEnabled";

    public const string CONFIG_KEY_ConnectionString = "DbConnString";
    public const string CONFIG_KEY_DbProvider = "Provider";

    public const string CONFIG_VALUE_DbProvider_SqlServer = "SqlServer";
    public const string CONFIG_VALUE_DbProvider_Postgres = "Postgres";
    public const string CONFIG_VALUE_DbProvider_Sqlite = "Sqlite";

    public const string CONFIG_KEY_JwtSettings_Key = "JwtSettings:Key";
    public const string CONFIG_KEY_JwtSettings_Issuer = "JwtSettings:Issuer";
    public const string CONFIG_KEY_JwtSettings_Audience = "JwtSettings:Audience";
    public const string CONFIG_KEY_JwtSettings_AccessTokenDurationSeconds = "JwtSettings:AccessTokenDurationSeconds";
    public const string CONFIG_KEY_JwtSettings_RefreshTokenDurationSeconds = "JwtSettings:RefreshTokenDurationSeconds";
    public const string CONFIG_KEY_JwtSettings_ClockSkewSeconds = "JwtSettings:ClockSkewSeconds";

    public const string CONFIG_KEY_SeedUsers_Admin_UserName = "SeedUsers:Admin:UserName";
    public const string CONFIG_KEY_SeedUsers_Admin_Password = "SeedUsers:Admin:Password";
    public const string CONFIG_KEY_SeedUsers_Admin_Email = "SeedUsers:Admin:Email";

}