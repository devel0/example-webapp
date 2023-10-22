namespace ExampleWebApp.Backend.WebApi;

public static partial class Extensions
{

    /// <summary>
    /// Helper to return an <see cref="ObjectResult"/> 
    /// within data and status included in then given result object <see cref="WebApi.ICommonResponseDto{T}"/>.
    /// </summary>
    /// <param name="result">Result returned by the api.</param>
    /// <returns><see cref="ActionResult{TValue}"/> returned by the api.</returns>
    public static ActionResult<T> CommonResponse<T>(this ControllerBase controller, ICommonResponseDto<T> result) =>
        controller.StatusCode((int)result.HttpStatus, result.Data);

    /// <summary>
    /// Helper to return an <see cref="ObjectResult"/> 
    /// within status included in then given result object <see cref="WebApi.ICommonResponseDto"/>.
    /// </summary>
    /// <param name="result">Result returned by the api.</param>
    /// <returns><see cref="ActionResult"/> returned by the api.</returns>
    public static ActionResult CommonResponse(this ControllerBase controller, ICommonResponseDto result) =>
        controller.StatusCode((int)result.HttpStatus, result);

}