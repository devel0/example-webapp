namespace ExampleWebApp.Backend.Tests.Integration;

public static class Extensions
{

    public static LoginRequestDto AdminLoginDto_onlyUsername(this IConfiguration configration) =>
        new LoginRequestDto
        {
            UserName = configration.GetConfigVar(CONFIG_KEY_SeedUsers_Admin_UserName),
            Password = configration.GetConfigVar(CONFIG_KEY_SeedUsers_Admin_Password),
        };

    public static LoginRequestDto AdminLoginDto_onlyEmail(this IConfiguration configration) =>
        new LoginRequestDto
        {
            Email = configration.GetConfigVar(CONFIG_KEY_SeedUsers_Admin_Email),
            Password = configration.GetConfigVar(CONFIG_KEY_SeedUsers_Admin_Password),
        };

    public static LoginRequestDto AdminLoginDto(this IConfiguration configration) =>
        new LoginRequestDto
        {
            UserName = configration.GetConfigVar(CONFIG_KEY_SeedUsers_Admin_UserName),
            Email = configration.GetConfigVar(CONFIG_KEY_SeedUsers_Admin_Email),
            Password = configration.GetConfigVar(CONFIG_KEY_SeedUsers_Admin_Password),
        };

    public static (TimeSpan accessTokenDuration, TimeSpan refreshTokenDuration, TimeSpan clockSkew)
        GetConfigurationJwtTimeout(this IConfiguration configuration)
    {
        var accessTokenDuration = TimeSpan.FromSeconds(int.Parse(configuration[CONFIG_KEY_JwtSettings_AccessTokenDurationSeconds]!));
        var refreshTokenDuration = TimeSpan.FromSeconds(int.Parse(configuration[CONFIG_KEY_JwtSettings_RefreshTokenDurationSeconds]!));
        var clockSkew = TimeSpan.FromSeconds(int.Parse(configuration[CONFIG_KEY_JwtSettings_ClockSkewSeconds]!));

        return (accessTokenDuration, refreshTokenDuration, clockSkew);
    }

    public static void SetJwtTimeout(this IConfiguration configuration,
        TimeSpan accessTokenDuration,
        TimeSpan refreshTokenDuration,
        TimeSpan clockSkew)
    {
        configuration[CONFIG_KEY_JwtSettings_AccessTokenDurationSeconds] = accessTokenDuration.TotalSeconds.ToString();
        configuration[CONFIG_KEY_JwtSettings_RefreshTokenDurationSeconds] = refreshTokenDuration.TotalSeconds.ToString();
        configuration[CONFIG_KEY_JwtSettings_ClockSkewSeconds] = clockSkew.TotalSeconds.ToString();
    }

    public static void SetXCookies(this HttpClient client, JwtCookies jwtCookies, bool clear = true)
    {
        client.DefaultRequestHeaders.Clear();
        client.SetUserName(jwtCookies.UserName!);
        client.SetAccessToken(jwtCookies.AccessToken!);
        client.SetRefreshToken(jwtCookies.RefreshToken!);
    }

    public static void SetUserName(this HttpClient client, string username)
    {
        client.DefaultRequestHeaders.Add(WEB_Cookie, $"{WEB_CookieName_XUsername}={username};");
    }

    public static void SetAccessToken(this HttpClient client, string accessToken)
    {
        client.DefaultRequestHeaders.Add(WEB_Cookie, $"{WEB_CookieName_XAccessToken}={accessToken};");
    }

    public static void SetRefreshToken(this HttpClient client, string refreshToken)
    {
        client.DefaultRequestHeaders.Add(WEB_Cookie, $"{WEB_CookieName_XRefreshToken}={refreshToken};");
    }

    public static JwtSecurityToken DecodeJwtAccessToken(string accessToken)
    {
        var jwtDecoded = DecodeToJwtSecurityToken(accessToken);
        if (jwtDecoded is null) throw new Exception("invalid jwt token");

        return jwtDecoded;
    }

    public static JwtSecurityToken GetJwtSecurityToken(this JwtCookies jwtCookies) =>
         DecodeJwtAccessToken(jwtCookies.AccessToken!);

}