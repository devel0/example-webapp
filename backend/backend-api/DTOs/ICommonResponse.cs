namespace ExampleWebApp.Backend.WebApi;

/// <summary>
/// Common response with data.
/// </summary>
public interface ICommonResponseDto<T> : ICommonResponseDto
{

    /// <summary>
    /// Custom data
    /// </summary>    
    T Data { get; }

}
