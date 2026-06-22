using IssueBoard.Application.Abstractions.Messaging;
using IssueBoard.Application.Abstractions.Persistence;
using IssueBoard.Application.Common.Errors;
using IssueBoard.Application.Dtos;
using IssueBoard.Application.Workspaces;
using IssueBoard.Domain.Entities;

namespace IssueBoard.Application.Projects.Get;

public sealed class GetProjectByIdQueryHandler
    : IQueryHandler<GetProjectByIdQuery, ProjectDto>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IWorkspaceRepository _workspaceRepository;

    public GetProjectByIdQueryHandler(
        IProjectRepository projectRepository,
        IWorkspaceRepository workspaceRepository)
    {
        _projectRepository = projectRepository;
        _workspaceRepository = workspaceRepository;
    }

    public async Task<Result<ProjectDto>> Handle(
        GetProjectByIdQuery request,
        CancellationToken cancellationToken)
    {
        Project? project = await _projectRepository.GetByIdAsync(
            request.ProjectId,
            cancellationToken);

        if (project is null)
        {
            return Result<ProjectDto>.Failure(
                ProjectErrors.NotFound(request.ProjectId));
        }

        Workspace? workspace = await _workspaceRepository.GetByIdAsync(
            project.WorkspaceId,
            cancellationToken);

        if (workspace is null)
        {
            return Result<ProjectDto>.Failure(
                ProjectErrors.WorkspaceNotFound(project.WorkspaceId));
        }

        if (!WorkspaceAuthorization.IsMember(workspace, request.RequesterUserId))
        {
            return Result<ProjectDto>.Failure(
                ProjectErrors.NotWorkspaceMember());
        }

        return Result<ProjectDto>.Success(project.ToDto());
    }
}
