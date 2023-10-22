namespace ExampleWebApp.Backend.Data.Types;

public class UserRefreshToken
{

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; }

    public required string UserName { get; set; }

    public required string RefreshToken { get; set; }

    /// <summary>
    /// issued utc timestamp
    /// </summary>
    public required DateTimeOffset Issued { get; set; }

    /// <summary>
    /// expires utc timestamp
    /// </summary>    
    public required DateTimeOffset Expires { get; set; }

}
