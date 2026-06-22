namespace IssueBoard.Web.Services;

public sealed class ApiResult<T>
{
    private ApiResult(
        bool isSuccess,
        T? value,
        string? error,
        int? statusCode)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
        StatusCode = statusCode;
    }

    public bool IsSuccess { get; }

    public T? Value { get; }

    public string? Error { get; }

    public int? StatusCode { get; }

    public static ApiResult<T> Success(T value)
    {
        return new ApiResult<T>(
            true,
            value,
            null,
            null);
    }

    public static ApiResult<T> Failure(
        string error,
        int? statusCode = null)
    {
        return new ApiResult<T>(
            false,
            default,
            error,
            statusCode);
    }
}
