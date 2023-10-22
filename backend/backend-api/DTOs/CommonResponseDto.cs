namespace ExampleWebApp.Backend.WebApi;

/// <summary>
/// Base class for <see cref="ICommonResponseDto"/> types.
/// </summary>
public class CommonResponseBaseDto : ICommonResponseDto
{

    /// <summary>
    /// Succeeded if http status code is <see cref="HttpStatusCode.OK"/>.
    /// </summary>
    public bool Succeded => HttpStatus == HttpStatusCode.OK;

    public HttpStatusCode HttpStatus { get; set; }

    [JsonConstructor]
    protected CommonResponseBaseDto()
    {
    }

    public CommonResponseBaseDto(HttpStatusCode status)
    {
        HttpStatus = status;
    }

}

/// <summary>
/// Base class for <see cref="ICommonResponseDto{T}"/> types.
/// </summary>
public class CommonResponseDto<T> : CommonResponseBaseDto, ICommonResponseDto<T> where T : class
{

    public T Data { get; set; }

    [JsonConstructor]
    CommonResponseDto()
    {
    }

    public CommonResponseDto(HttpStatusCode status, T data) : base(status)
    {
        Data = data;
    }

}
