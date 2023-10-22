namespace ExampleWebApp.Backend.Data;

public partial class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions options)
        : base(options)
    {

    }    

    public required DbSet<UserRefreshToken> UserRefreshTokens { get; set; }

    public required DbSet<SomeData> SomeDatas { get; set; }

}
