namespace ExampleWebApp.Backend.WebApi;

public static partial class Extensions
{

    /// <summary>
    /// Logger factory for db sensitive data logging.
    /// Used if <see cref="CONFIG_KEY_DbLoggingEnabled"/> config key set to true.
    /// </summary>    
    static readonly ILoggerFactory loggerFactory = LoggerFactory.Create(loggingBuilder => loggingBuilder
        .SetMinimumLevel(LogLevel.Information)
        .AddConsole());

    /// <summary>
    /// Configure db connection string and provider.
    /// </summary>    
    public static void ConfigureDatabase(this WebApplicationBuilder webApplicationBuilder)
    {
        if (IsUnitTesting())
        {
            webApplicationBuilder.Configuration["Logging:LogLevel:Default"] = "Warning";
            webApplicationBuilder.Configuration[CONFIG_KEY_DbProvider] = "Sqlite";
            webApplicationBuilder.Configuration[CONFIG_KEY_ConnectionString] = "Data Source=:memory:";
        }

        var connString = webApplicationBuilder.Configuration.GetConfigVar(CONFIG_KEY_ConnectionString);
        var provider = webApplicationBuilder.Configuration.GetConfigVar(CONFIG_KEY_DbProvider);
        var dbLoggingEnabled = webApplicationBuilder.Configuration.GetConfigVar<bool>(CONFIG_KEY_DbLoggingEnabled);

        //
        // specific config for sqlite in memory
        //
        if (provider == CONFIG_VALUE_DbProvider_Sqlite)
        {
            if (connString.Contains(":memory:"))
            {
                var services = webApplicationBuilder.Services;

                services.AddSingleton<DbConnection>(container =>
                {
                    var connection = new SqliteConnection(connString);
                    if (connection is null) throw new Exception($"can't open sqlite in memory");
                    connection.Open();

                    return connection;
                });

                services.AddDbContext<ApplicationDbContext>((container, options) =>
                {
                    var connection = container.GetRequiredService<DbConnection>();
                    options.UseSqlite(connection, x => x.MigrationsAssembly("migrations-sqlite"));
                });

                return;
            }
        }

        //
        // normal config for db providers
        //
        webApplicationBuilder.Services.AddDbContext<ApplicationDbContext>(options =>
        {
            switch (provider)
            {

                case CONFIG_VALUE_DbProvider_Postgres:
                    {
                        options.UseNpgsql(connString, x => x.MigrationsAssembly("migrations-psql"));
                    }
                    break;

                case CONFIG_VALUE_DbProvider_SqlServer:
                    {
                        if (webApplicationBuilder.Environment.IsDevelopment())
                        {
                            if (!connString.EndsWith(';')) connString += ";";

                            connString += "Encrypt=false";
                        }
                        options.UseSqlServer(connString, x => x.MigrationsAssembly("migrations-mssql"));
                    }
                    break;

                case CONFIG_VALUE_DbProvider_Sqlite:
                    {
                        options.UseSqlite(connString, x => x.MigrationsAssembly("migrations-sqlite"));
                    }
                    break;
            }

            if (dbLoggingEnabled)
            {
                options
                    .UseLoggerFactory(loggerFactory)
                    .EnableSensitiveDataLogging();
            }

        });
    }

    /// <summary>
    /// Auto apply initial migration.
    /// </summary>    
    public static void ApplyDatabaseInitialMigration(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var appliedMigrations = db.Database.GetAppliedMigrations().ToList();
        var pendingMigrations = db.Database.GetPendingMigrations().ToList();

        if (appliedMigrations.Count == 0 && pendingMigrations.Count == 0)
            throw new Exception("no initial migration found");

        if (appliedMigrations.Count == 0 && pendingMigrations.Count > 0)
        {
            if (!IsUnitTesting())
            {
                app.Logger.LogInformation("Apply initial database migration");
            }
            db.Database.GetInfrastructure().GetService<IMigrator>()?.Migrate(pendingMigrations[0]);

            if (IsUnitTesting()) // apply further migrations only on unit testing
            {
                for (int i = 1; i < pendingMigrations.Count; ++i)
                    db.Database.GetInfrastructure().GetService<IMigrator>()?.Migrate(pendingMigrations[i]);
            }
        }
    }

}