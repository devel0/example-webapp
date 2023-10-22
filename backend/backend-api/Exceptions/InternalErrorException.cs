namespace ExampleWebApp.Backend.WebApi;

/// <summary>
/// Internal error exception. This should not happen.
/// </summary>
public class InternalErrorException : Exception
{

    public InternalErrorException(string msg) : base(msg)
    {
    }

}
