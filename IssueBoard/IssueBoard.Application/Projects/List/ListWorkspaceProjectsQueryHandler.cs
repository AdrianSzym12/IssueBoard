using IssueBoard.Application.Abstractions.Messaging;
using IssueBoard.Application.Abstractions.Persistence;
using IssueBoard.Application.Common.Errors;
using IssueBoard.Application.Dtos;
using IssueBoard.Application.Workspaces;
using IssueBoard.Domain.Entities;

namespace IssueBoard.Application.Projects.List;

public sealed class ListWorkspaceProjectsQueryHandler
    : IQueryHandler<ListWorkspaceProjectsQuery, IReadOnlyList<ProjectDto>>
{
    private readonly IWorkspaceRepository _workspaceRepository;
    private readonly IProjectRepository _projectRepository;

    public ListWorkspaceProjectsQueryHandler(
        IWorkspaceRepository workspaceRepository,
        IProjectRepository projectRepository)
    {
        _workspaceRepository = workspaceRepository;
        _projectRepository = projectRepository;
    }

    public async Task<Result<IReadOnlyList<ProjectDto>>> Handle(
        ListWorkspaceProjectsQuery request,
        CancellationToken cancellationToken)
    {
        Workspace? workspace = await _workspaceRepository.GetByIdAsync(
            request.WorkspaceId,
            cancellationToken);

        if (workspace is null)
        {
            return Result<IReadOnlyList<ProjectDto>>.Failure(
                ProjectErrors.WorkspaceNotFound(request.WorkspaceId));
        }

        if (!WorkspaceAuthorization.IsMember(workspace, request.RequesterUserId))
        {
            return Result<IReadOnlyList<ProjectDto>>.Failure(
                ProjectErrors.NotWorkspaceMember());
        }

        IReadOnlyList<Project> projects = await _projectRepository.ListByWorkspaceIdAsync(
            request.WorkspaceId,
            request.IncludeArchived,
            cancellationToken);

        IReadOnlyList<ProjectDto> response = projects
            .Select(project => project.ToDto())
            .ToList();

        return Result<IReadOnlyList<ProjectDto>>.Success(response);
    }
}
