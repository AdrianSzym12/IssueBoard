using IssueBoard.Web.Models.Workspaces;

namespace IssueBoard.Web.Services;

public sealed class WorkspaceService : IWorkspaceService
{
    private readonly IApiClient _apiClient;

    public WorkspaceService(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<ApiResult<List<WorkspaceDto>>> ListAsync(
        CancellationToken cancellationToken = default)
    {
        return _apiClient.GetAsync<List<WorkspaceDto>>(
            "api/workspaces",
            cancellationToken);
    }

    public Task<ApiResult<WorkspaceDto>> CreateAsync(
        CreateWorkspaceRequest request,
        CancellationToken cancellationToken = default)
    {
        return _apiClient.PostAsync<CreateWorkspaceRequest, WorkspaceDto>(
            "api/workspaces",
            request,
            cancellationToken);
    }
}
