namespace ExampleWebApp.Backend.Data.Types;

public class AuditableEntity
{

    [Key]
    public int Id { get; set; }

    public DateTimeOffset CreateTimestamp { get; set; }
    public DateTimeOffset? UpdateTimestamp { get; set; }

    public AuditableEntity()
    {        
    }

}
