namespace ExampleWebApp.Backend.WebApi;

public static partial class Extensions
{

    /// <summary>
    /// Helper to retrieve configuration from environment or appsettings if found.
    /// From the environment the same config key variable will be searched within colon replaced by underline.
    /// </summary>
    /// <param name="path">Variable path (ie. <see cref="CONFIG_KEY_JwtSettings_Key"/></param>    
    public static string GetConfigVar(this IConfiguration configuration, string path) =>
        configuration.GetConfigVar<string>(path);

    /// <summary>
    /// Typed helper to retrieve configuration from environment or appsettings if found.
    /// From the environment the same config key variable will be searched within colon replaced by underline.
    /// </summary>
    /// <param name="path">Variable path (ie. <see cref="CONFIG_KEY_JwtSettings_Key"/></param>    
    public static T GetConfigVar<T>(this IConfiguration configuration, string path)
    {
        object? envVar = Environment.GetEnvironmentVariable(path.Replace(":", "_"));
        if (envVar is not null)
        {
            var cvt = TypeDescriptor.GetConverter(envVar);
            var q = cvt.CanConvertTo(typeof(T));
            if (q)
            {
                var res = cvt.ConvertTo(envVar, typeof(T));
                if (res is T t)
                    return t;
            }
        }

        var value = configuration.GetValue<T>(path);
        if (value is null) throw new Exception($"Unable to find value for config var [{path}]. Check your user secrets.");

        return value;
    }

}