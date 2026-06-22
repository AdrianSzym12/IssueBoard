using IssueBoard.Web.Models.Projects;

namespace IssueBoard.Web.Services;

public sealed class ProjectService : IProjectService
{
    private readonly IApiClient _apiClient;

    public ProjectService(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public Task<ApiResult<List<ProjectDto>>> ListAsync(
        Guid workspaceId,
        CancellationToken cancellationToken = default)
    {
        return _apiClient.GetAsync<List<ProjectDto>>(
            $"api/workspaces/{workspaceId}/projects",
            cancellationToken);
    }

    public Task<ApiResult<ProjectDto>> CreateAsync(
        Guid workspaceId,
        CreateProjectRequest request,
        CancellationToken cancellationToken = default)
    {
        return _apiClient.PostAsync<CreateProjectRequest, ProjectDto>(
            $"api/workspaces/{workspaceId}/projects",
            request,
            cancellationToken);
    }
}
