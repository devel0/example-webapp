namespace ExampleWebApp.Backend.Tests.Integration;

public class FactoryHelper
{
    public WebApplicationFactory<Program> Program { get; }    

    public HttpClient NewClient() => Program.CreateClient();

    public IConfiguration Configuration { get; }
    public IOptions<JsonOptions> JsonOptions { get; }

    internal ILogger<Program> logger { get; }

    public FactoryHelper(CustomWebApplicationFactory<Program> factory)
    {
        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(ENV_VARNAME_ASPNETCORE_ENVIRONMENT)))
            Environment.SetEnvironmentVariable(ENV_VARNAME_ASPNETCORE_ENVIRONMENT, TEST_ENV_ASPNETCORE_ENVIRONMENT);

        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(ENV_VARNAME_ASPNETCORE_URLS)))
            Environment.SetEnvironmentVariable(ENV_VARNAME_ASPNETCORE_URLS, TEST_ENV_ASPNETCORE_URLS);

        Program = factory
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {

                });
            });

        var sp = factory.Services.GetRequiredService<IServiceProvider>();

        logger = sp.GetRequiredService<ILogger<Program>>();

        Configuration = Program.Services.GetRequiredService<IConfiguration>();
    }
}
