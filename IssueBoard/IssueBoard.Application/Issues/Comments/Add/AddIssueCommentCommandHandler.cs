using IssueBoard.Application.Abstractions.Messaging;
using IssueBoard.Application.Abstractions.Persistence;
using IssueBoard.Application.Common.Errors;
using IssueBoard.Application.Dtos;
using IssueBoard.Domain.Entities;

namespace IssueBoard.Application.Issues.Comments.Add;

public sealed class AddIssueCommentCommandHandler
    : ICommandHandler<AddIssueCommentCommand, IssueCommentDto>
{
    private readonly IIssueRepository _issueRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IWorkspaceRepository _workspaceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddIssueCommentCommandHandler(
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

    public async Task<Result<IssueCommentDto>> Handle(
        AddIssueCommentCommand request,
        CancellationToken cancellationToken)
    {
        Issue? issue = await _issueRepository.GetByIdAsync(
            request.IssueId,
            cancellationToken);

        if (issue is null)
        {
            return Result<IssueCommentDto>.Failure(
                IssueErrors.NotFound(request.IssueId));
        }

        Project? project = await _projectRepository.GetByIdAsync(
            issue.ProjectId,
            cancellationToken);

        if (project is null)
        {
            return Result<IssueCommentDto>.Failure(
                IssueErrors.ProjectNotFound(issue.ProjectId));
        }

        if (project.IsArchived)
        {
            return Result<IssueCommentDto>.Failure(
                IssueErrors.ProjectArchived(project.Id));
        }

        Workspace? workspace = await _workspaceRepository.GetByIdAsync(
            project.WorkspaceId,
            cancellationToken);

        if (workspace is null)
        {
            return Result<IssueCommentDto>.Failure(
                IssueErrors.WorkspaceNotFound(project.WorkspaceId));
        }

        if (!IssueAuthorization.CanManageIssues(workspace, request.RequesterUserId))
        {
            return Result<IssueCommentDto>.Failure(
                IssueErrors.MissingPermission());
        }

        IssueComment comment = issue.AddComment(
            request.Content,
            request.RequesterUserId);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<IssueCommentDto>.Success(comment.ToDto());
    }
}
