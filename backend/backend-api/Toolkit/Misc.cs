namespace ExampleWebApp.Backend.WebApi;

public static partial class Toolkit
{

    /// <summary>
    /// States if execution is for unit testing by evaluating if the <see cref="ENV_UNIT_TESTING"/> env var equals to "1".
    /// </summary>
    public static bool IsUnitTesting() => Environment.GetEnvironmentVariable(ENV_UNIT_TESTING) is string str && str == "1";

}