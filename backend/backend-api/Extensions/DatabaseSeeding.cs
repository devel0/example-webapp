namespace ExampleWebApp.Backend.WebApi;

public static partial class Extensions
{

    /// <summary>
    /// Seed database admin user and roles.
    /// </summary>
    public static async Task InitializeDatabaseAsync(this WebApplication webApplication)
    {
        using var scope = webApplication.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<Serilog.ILogger>();
        var usermgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var rolemgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var config = webApplication.Configuration;

        var adminUserName = config.GetConfigVar(CONFIG_KEY_SeedUsers_Admin_UserName);
        var adminEmail = config.GetConfigVar(CONFIG_KEY_SeedUsers_Admin_Email);
        var adminPassword = config.GetConfigVar(CONFIG_KEY_SeedUsers_Admin_Password);

        if (db.Users.Count() == 0)
        {
            var user = new ApplicationUser
            {
                UserName = adminUserName,
                Email = adminEmail
            };

            var res = await usermgr.CreateAsync(user, adminPassword);
            if (!res.Succeeded)
                throw new Exception($"Unable to create initial user {string.Join(";", res.Errors.Select(w => w.Description))}");

            await rolemgr.CreateAsync(new IdentityRole(ROLE_admin));
            await rolemgr.CreateAsync(new IdentityRole(ROLE_user));

            await usermgr.AddToRoleAsync(user, ROLE_admin);
            var confirmEmailToken = await usermgr.GenerateEmailConfirmationTokenAsync(user);
            await usermgr.ConfirmEmailAsync(user, confirmEmailToken);

            if (!IsUnitTesting())
                logger.Information($"Created admin");
        }

    }

}