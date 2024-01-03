namespace ExampleWebApp.Backend.WebApi;

public static partial class Extensions
{

    /// <summary>
    /// Add cors, in development environment, for given corsAllowedOrigins.
    /// </summary>    
    public static void SetupSecurityPolicies(this WebApplicationBuilder webApplicationBuilder,
        string? corsAllowedOrigins)
    {
        if (webApplicationBuilder.Environment.IsDevelopment() && corsAllowedOrigins is not null)
        {
            webApplicationBuilder.Services.AddCors(options =>
            {
                options.AddPolicy(APP_DevelopmentCorsPolicyName, policy =>
                {
                    policy
                        .WithOrigins(corsAllowedOrigins.Split(','))
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });
        }
    }

    /// <summary>
    /// Sets https redirection in production and Cors in development.
    /// Adds authentication middleware.
    /// Adds authorization middleware.
    /// </summary>
    public static void SetupSecurityPolicies(this WebApplication webApplication)
    {
        if (!webApplication.Environment.IsDevelopment()) // production
        {
            webApplication.UseHttpsRedirection();

        }

        else // development
        {
            webApplication.UseCors(APP_DevelopmentCorsPolicyName);
        }

        webApplication.UseAuthentication();
        webApplication.UseAuthorization();
    }

}