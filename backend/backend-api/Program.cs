var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

// merge user secrets to app configuration
builder.Configuration.AddUserSecrets(Assembly.GetExecutingAssembly());

builder.ConfigureDatabase();

builder.SetupApplicationCookie();
builder.Services.SetupIdentityProvider();
builder.SetupSecurityPolicies(corsAllowedOrigins: "http://localhost:3000");
builder.SetupJWTAuthentication();
builder.Services.AddScoped<IJWTService, JWTService>(); // add JWT service used by auth service
builder.Services.AddScoped<IAuthService, AuthService>(); // add auth service used by the auth controller

builder.Services.AddControllers(options =>
{
    options.Filters.Add<JWTFilter>();
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGenCustom();

//----------------------------------------------------------------------------------
// APP BUILD
//----------------------------------------------------------------------------------
var app = builder.Build();

var swaggerNfo = app.ConfigSwagger();
app.SetupSecurityPolicies();
app.MapControllers();
app.ApplyDatabaseInitialMigration();
await app.InitializeDatabaseAsync();
await app.UpgradeRolesAsync();

//----------------------------------------------------------------------------------
// APP START
//----------------------------------------------------------------------------------

await app.StartAsync();

if (!IsUnitTesting())
    app.Logger.LogInformation($"Listening on {string.Join(" ", app.Urls.Select(w => w.ToString()))}");

if (swaggerNfo is not null)
{
    var httpUrl = app.Urls.FirstOrDefault(w => w.StartsWith("http:"));
    if (httpUrl is not null && !IsUnitTesting())
        app.Logger.LogInformation($"Swagger UI available on {httpUrl}{swaggerNfo.Value.swaggerUIPath}");
}

await app.WaitForShutdownAsync();

app.Run();

// partial Program for unit testing
public partial class Program { }