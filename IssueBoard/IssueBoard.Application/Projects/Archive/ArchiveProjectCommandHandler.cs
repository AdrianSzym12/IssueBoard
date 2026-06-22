using IssueBoard.Application.Abstractions.Messaging;
using IssueBoard.Application.Abstractions.Persistence;
using IssueBoard.Application.Common.Errors;
using IssueBoard.Application.Dtos;
using IssueBoard.Application.Workspaces;
using IssueBoard.Domain.Entities;

namespace IssueBoard.Application.Projects.Archive;

public sealed class ArchiveProjectCommandHandler
    : ICommandHandler<ArchiveProjectCommand, ProjectDto>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IWorkspaceRepository _workspaceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ArchiveProjectCommandHandler(
        IProjectRepository projectRepository,
        IWorkspaceRepository workspaceRepository,
        IUnitOfWork unitOfWork)
    {
        _projectRepository = projectRepository;
        _workspaceRepository = workspaceRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ProjectDto>> Handle(
        ArchiveProjectCommand request,
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

        if (!WorkspaceAuthorization.CanManageMembers(workspace, request.RequesterUserId))
        {
            return Result<ProjectDto>.Failure(
                ProjectErrors.MissingPermission());
        }

        project.Archive();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<ProjectDto>.Success(project.ToDto());
    }
}
