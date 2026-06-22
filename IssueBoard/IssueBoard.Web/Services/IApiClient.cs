namespace IssueBoard.Web.Services;

public interface IApiClient
{
    Task<ApiResult<TResponse>> GetAsync<TResponse>(
        string url,
        CancellationToken cancellationToken = default);

    Task<ApiResult<TResponse>> PostAsync<TRequest, TResponse>(
        string url,
        TRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiResult<TResponse>> PutAsync<TRequest, TResponse>(
        string url,
        TRequest request,
        CancellationToken cancellationToken = default);

    Task<ApiResult<bool>> DeleteAsync(
        string url,
        CancellationToken cancellationToken = default);
}
