namespace ExampleWebApp.Backend.Tests.Integration;

public class CustomWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("UnitTesting", "1");
        builder.UseEnvironment("Development");                        
    }
   
}
