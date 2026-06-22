using IssueBoard.Web.Models.Projects;

namespace IssueBoard.Web.Services;

public interface IProjectService
{
    Task<ApiResult<List<ProjectDto>>> ListAsync(
        Guid workspaceId,
        CancellationToken cancellationToken = default);

    Task<ApiResult<ProjectDto>> CreateAsync(
        Guid workspaceId,
        CreateProjectRequest request,
        CancellationToken cancellationToken = default);
}
