using IssueBoard.Web.Models.Common;
using IssueBoard.Web.Models.Issues;

namespace IssueBoard.Web.Services;

public sealed class IssueService : IIssueService
{
    private readonly IApiClient _apiClient;

    public IssueService(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<ApiResult<PagedList<IssueDto>>> SearchAsync(
        Guid projectId,
        SearchIssuesRequest request,
        CancellationToken cancellationToken = default)
    {
        string url = BuildSearchUrl(projectId, request);

        return _apiClient.GetAsync<PagedList<IssueDto>>(
            url,
            cancellationToken);
    }

    public Task<ApiResult<IssueDto>> CreateAsync(
        Guid projectId,
        CreateIssueRequest request,
        CancellationToken cancellationToken = default)
    {
        return _apiClient.PostAsync<CreateIssueRequest, IssueDto>(
            $"api/projects/{projectId}/issues",
            request,
            cancellationToken);
    }

    public Task<ApiResult<IssueDto>> ChangeStatusAsync(
        Guid issueId,
        ChangeIssueStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        return _apiClient.PutAsync<ChangeIssueStatusRequest, IssueDto>(
            $"api/issues/{issueId}/status",
            request,
            cancellationToken);
    }

    private static string BuildSearchUrl(
        Guid projectId,
        SearchIssuesRequest request)
    {
        List<string> query = [];

        if (request.Status.HasValue)
        {
            query.Add($"status={request.Status.Value}");
        }

        if (request.Priority.HasValue)
        {
            query.Add($"priority={request.Priority.Value}");
        }

        if (request.AssigneeUserId.HasValue)
        {
            query.Add($"assigneeUserId={request.AssigneeUserId.Value}");
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query.Add($"searchTerm={Uri.EscapeDataString(request.SearchTerm)}");
        }

        if (!string.IsNullOrWhiteSpace(request.SortBy))
        {
            query.Add($"sortBy={Uri.EscapeDataString(request.SortBy)}");
        }

        query.Add($"sortDescending={request.SortDescending.ToString().ToLowerInvariant()}");
        query.Add($"pageNumber={request.PageNumber}");
        query.Add($"pageSize={request.PageSize}");

        string queryString = string.Join("&", query);

        return $"api/projects/{projectId}/issues?{queryString}";
    }
}
