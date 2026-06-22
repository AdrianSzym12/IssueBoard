using IssueBoard.Application.Abstractions.Messaging;
using IssueBoard.Application.Abstractions.Persistence;
using IssueBoard.Application.Common.Errors;
using IssueBoard.Application.Dtos;
using IssueBoard.Domain.Entities;

namespace IssueBoard.Application.Issues.Comments.List;

public sealed class ListIssueCommentsQueryHandler
    : IQueryHandler<ListIssueCommentsQuery, IReadOnlyList<IssueCommentDto>>
{
    private readonly IIssueRepository _issueRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IWorkspaceRepository _workspaceRepository;

    public ListIssueCommentsQueryHandler(
        IIssueRepository issueRepository,
        IProjectRepository projectRepository,
        IWorkspaceRepository workspaceRepository)
    {
        _issueRepository = issueRepository;
        _projectRepository = projectRepository;
        _workspaceRepository = workspaceRepository;
    }

    public async Task<Result<IReadOnlyList<IssueCommentDto>>> Handle(
        ListIssueCommentsQuery request,
        CancellationToken cancellationToken)
    {
        Issue? issue = await _issueRepository.GetByIdAsync(
            request.IssueId,
            cancellationToken);

        if (issue is null)
        {
            return Result<IReadOnlyList<IssueCommentDto>>.Failure(
                IssueErrors.NotFound(request.IssueId));
        }

        Project? project = await _projectRepository.GetByIdAsync(
            issue.ProjectId,
            cancellationToken);

        if (project is null)
        {
            return Result<IReadOnlyList<IssueCommentDto>>.Failure(
                IssueErrors.ProjectNotFound(issue.ProjectId));
        }

        Workspace? workspace = await _workspaceRepository.GetByIdAsync(
            project.WorkspaceId,
            cancellationToken);

        if (workspace is null)
        {
            return Result<IReadOnlyList<IssueCommentDto>>.Failure(
                IssueErrors.WorkspaceNotFound(project.WorkspaceId));
        }

        if (!IssueAuthorization.CanViewIssues(workspace, request.RequesterUserId))
        {
            return Result<IReadOnlyList<IssueCommentDto>>.Failure(
                IssueErrors.NotWorkspaceMember());
        }

        IReadOnlyList<IssueCommentDto> comments = issue.Comments
            .OrderBy(comment => comment.CreatedAtUtc)
            .Select(comment => comment.ToDto())
            .ToList();

        return Result<IReadOnlyList<IssueCommentDto>>.Success(comments);
    }
}
