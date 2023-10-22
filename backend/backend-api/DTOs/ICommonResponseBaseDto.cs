namespace ExampleWebApp.Backend.WebApi;

/// <summary>
/// Common api response.
/// </summary>
public interface ICommonResponseDto
{

    /// <summary>
    /// Http status result.
    /// This will forward by the <see cref="Extensions.CommonResponse(ControllerBase, ICommonResponseDto)"/>  
    /// and <see cref="Extensions.CommonResponse{T}(ControllerBase, ICommonResponseDto{T})"/> helpers.
    /// </summary>    
    HttpStatusCode HttpStatus { get; }

}
