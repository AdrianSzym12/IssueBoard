using IssueBoard.Application.Abstractions.Messaging;
using IssueBoard.Application.Abstractions.Persistence;
using IssueBoard.Application.Common.Errors;
using IssueBoard.Application.Dtos;
using IssueBoard.Domain.Entities;

namespace IssueBoard.Application.Issues.Assign;

public sealed class AssignIssueCommandHandler
    : ICommandHandler<AssignIssueCommand, IssueDto>
{
    private readonly IIssueRepository _issueRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IWorkspaceRepository _workspaceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AssignIssueCommandHandler(
        IIssueRepository issueRepository,
        IProjectRepository projectRepository,
        IWorkspaceRepository workspaceRepository,
        IUnitOfWork unitOfWork)
    {
        _issueRepository = issueRepository;
        _projectRepository = projectRepository;
        _workspaceRepository = workspaceRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IssueDto>> Handle(
        AssignIssueCommand request,
        CancellationToken cancellationToken)
    {
        Issue? issue = await _issueRepository.GetByIdAsync(
            request.IssueId,
            cancellationToken);

        if (issue is null)
        {
            return Result<IssueDto>.Failure(
                IssueErrors.NotFound(request.IssueId));
        }

        Project? project = await _projectRepository.GetByIdAsync(
            issue.ProjectId,
            cancellationToken);

        if (project is null)
        {
            return Result<IssueDto>.Failure(
                IssueErrors.ProjectNotFound(issue.ProjectId));
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

        if (request.AssigneeUserId.HasValue)
        {
            if (!workspace.HasMember(request.AssigneeUserId.Value))
            {
                return Result<IssueDto>.Failure(
                    IssueErrors.AssigneeNotWorkspaceMember(request.AssigneeUserId.Value));
            }

            issue.AssignTo(request.AssigneeUserId.Value, request.RequesterUserId);
        }
        else
        {
            issue.Unassign(request.RequesterUserId);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<IssueDto>.Success(issue.ToDto());
    }
}
