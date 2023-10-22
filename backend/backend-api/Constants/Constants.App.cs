namespace ExampleWebApp.Backend.WebApi;

public static partial class Constants
{

    /// <summary>
    /// name of the cors policy created if in development mode
    /// </summary>
    public const string APP_DevelopmentCorsPolicyName = "development";

    public const string JWT_SecurityAlghoritm = SecurityAlgorithms.HmacSha256;

    public const string ENV_VARNAME_ASPNETCORE_ENVIRONMENT = "ASPNETCORE_ENVIRONMENT";
    public const string ENV_VARNAME_ASPNETCORE_URLS = "ASPNETCORE_URLS";

    public const string ENV_UNIT_TESTING = "UnitTesting";

}