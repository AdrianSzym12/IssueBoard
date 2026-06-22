using IssueBoard.Web.Models.Workspaces;

namespace IssueBoard.Web.Services;

public interface IWorkspaceService
{
    Task<ApiResult<List<WorkspaceDto>>> ListAsync(
        CancellationToken cancellationToken = default);

    Task<ApiResult<WorkspaceDto>> CreateAsync(
        CreateWorkspaceRequest request,
        CancellationToken cancellationToken = default);
}
