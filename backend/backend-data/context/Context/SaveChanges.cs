namespace ExampleWebApp.Backend.Data;

public partial class ApplicationDbContext
{

    void UpdateAuditableEntities()
    {
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is AuditableEntity auditableEntity)
            {
                if (entry.State == EntityState.Added)
                    auditableEntity.CreateTimestamp = DateTimeOffset.UtcNow;

                else if (entry.State == EntityState.Modified)
                    auditableEntity.UpdateTimestamp = DateTimeOffset.UtcNow;
            }
        }
    }

    public override int SaveChanges()
    {
        UpdateAuditableEntities();

        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        UpdateAuditableEntities();

        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

}
