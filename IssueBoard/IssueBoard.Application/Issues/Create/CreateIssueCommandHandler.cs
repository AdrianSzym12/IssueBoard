using IssueBoard.Application.Abstractions.Messaging;
using IssueBoard.Application.Abstractions.Persistence;
using IssueBoard.Application.Common.Errors;
using IssueBoard.Application.Dtos;
using IssueBoard.Domain.Entities;

namespace IssueBoard.Application.Issues.Create;

public sealed class CreateIssueCommandHandler
    : ICommandHandler<CreateIssueCommand, IssueDto>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IWorkspaceRepository _workspaceRepository;
    private readonly IIssueRepository _issueRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateIssueCommandHandler(
        IProjectRepository projectRepository,
        IWorkspaceRepository workspaceRepository,
        IIssueRepository issueRepository,
        IUnitOfWork unitOfWork)
    {
        _projectRepository = projectRepository;
        _workspaceRepository = workspaceRepository;
        _issueRepository = issueRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IssueDto>> Handle(
        CreateIssueCommand request,
        CancellationToken cancellationToken)
    {
        Project? project = await _projectRepository.GetByIdAsync(
            request.ProjectId,
            cancellationToken);

        if (project is null)
        {
            return Result<IssueDto>.Failure(
                IssueErrors.ProjectNotFound(request.ProjectId));
        }

        if (project.IsArchived)
        {
            return Result<IssueDto>.Failure(
                IssueErrors.ProjectArchived(project.Id));
        }

        Workspace? workspace = await _workspaceRepository.GetByIdAsync(
            project.WorkspaceId,
            cancellationToken);

        if (workspace is null)
        {
            return Result<IssueDto>.Failure(
                IssueErrors.WorkspaceNotFound(project.WorkspaceId));
        }

        if (!IssueAuthorization.CanManageIssues(workspace, request.RequesterUserId))
        {
            return Result<IssueDto>.Failure(
                IssueErrors.MissingPermission());
        }

        if (request.AssigneeUserId.HasValue &&
            !workspace.HasMember(request.AssigneeUserId.Value))
        {
            return Result<IssueDto>.Failure(
                IssueErrors.AssigneeNotWorkspaceMember(request.AssigneeUserId.Value));
        }

        int nextIssueNumber = await _issueRepository.GetNextIssueNumberAsync(
            project.Id,
            cancellationToken);

        Issue issue = Issue.Create(
            project.Id,
            nextIssueNumber,
            request.Title,
            request.Description,
            request.Priority,
            request.RequesterUserId,
            request.DueDateUtc);

        if (request.AssigneeUserId.HasValue)
        {
            issue.AssignTo(request.AssigneeUserId.Value, request.RequesterUserId);
        }

        _issueRepository.Add(issue);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<IssueDto>.Success(issue.ToDto());
    }
}
