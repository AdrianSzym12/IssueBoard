using IssueBoard.Application.Abstractions.Messaging;
using IssueBoard.Application.Abstractions.Persistence;
using IssueBoard.Application.Common.Errors;
using IssueBoard.Application.Dtos;
using IssueBoard.Application.Workspaces;
using IssueBoard.Domain.Entities;

namespace IssueBoard.Application.Projects.Create;

public sealed class CreateProjectCommandHandler
    : ICommandHandler<CreateProjectCommand, ProjectDto>
{
    private readonly IWorkspaceRepository _workspaceRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProjectCommandHandler(
        IWorkspaceRepository workspaceRepository,
        IProjectRepository projectRepository,
        IUnitOfWork unitOfWork)
    {
        _workspaceRepository = workspaceRepository;
        _projectRepository = projectRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ProjectDto>> Handle(
        CreateProjectCommand request,
        CancellationToken cancellationToken)
    {
        Workspace? workspace = await _workspaceRepository.GetByIdAsync(
            request.WorkspaceId,
            cancellationToken);

        if (workspace is null)
        {
            return Result<ProjectDto>.Failure(
                ProjectErrors.WorkspaceNotFound(request.WorkspaceId));
        }

        if (!WorkspaceAuthorization.CanManageMembers(workspace, request.RequesterUserId))
        {
            return Result<ProjectDto>.Failure(ProjectErrors.MissingPermission());
        }

        Project? existingProject = await _projectRepository.GetByWorkspaceAndKeyAsync(
            request.WorkspaceId,
            request.Key,
            cancellationToken);

        if (existingProject is not null)
        {
            return Result<ProjectDto>.Failure(
                ProjectErrors.KeyAlreadyExists(request.Key.ToUpperInvariant()));
        }

        Project project = Project.Create(
            request.WorkspaceId,
            request.Name,
            request.Key,
            request.Description);

        _projectRepository.Add(project);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<ProjectDto>.Success(project.ToDto());
    }
}
