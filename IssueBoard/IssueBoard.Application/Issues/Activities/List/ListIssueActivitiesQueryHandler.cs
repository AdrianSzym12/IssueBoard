using IssueBoard.Application.Abstractions.Messaging;
using IssueBoard.Application.Abstractions.Persistence;
using IssueBoard.Application.Common.Errors;
using IssueBoard.Application.Dtos;
using IssueBoard.Domain.Entities;

namespace IssueBoard.Application.Issues.Activities.List;

public sealed class ListIssueActivitiesQueryHandler
    : IQueryHandler<ListIssueActivitiesQuery, IReadOnlyList<IssueActivityDto>>
{
    private readonly IIssueRepository _issueRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IWorkspaceRepository _workspaceRepository;

    public ListIssueActivitiesQueryHandler(
        IIssueRepository issueRepository,
        IProjectRepository projectRepository,
        IWorkspaceRepository workspaceRepository)
    {
        _issueRepository = issueRepository;
        _projectRepository = projectRepository;
        _workspaceRepository = workspaceRepository;
    }

    public async Task<Result<IReadOnlyList<IssueActivityDto>>> Handle(
        ListIssueActivitiesQuery request,
        CancellationToken cancellationToken)
    {
        Issue? issue = await _issueRepository.GetByIdAsync(
            request.IssueId,
            cancellationToken);

        if (issue is null)
        {
            return Result<IReadOnlyList<IssueActivityDto>>.Failure(
                IssueErrors.NotFound(request.IssueId));
        }

        Project? project = await _projectRepository.GetByIdAsync(
            issue.ProjectId,
            cancellationToken);

        if (project is null)
        {
            return Result<IReadOnlyList<IssueActivityDto>>.Failure(
                IssueErrors.ProjectNotFound(issue.ProjectId));
        }

        Workspace? workspace = await _workspaceRepository.GetByIdAsync(
            project.WorkspaceId,
            cancellationToken);

        if (workspace is null)
        {
            return Result<IReadOnlyList<IssueActivityDto>>.Failure(
                IssueErrors.WorkspaceNotFound(project.WorkspaceId));
        }

        if (!IssueAuthorization.CanViewIssues(workspace, request.RequesterUserId))
        {
            return Result<IReadOnlyList<IssueActivityDto>>.Failure(
                IssueErrors.NotWorkspaceMember());
        }

        IReadOnlyList<IssueActivityDto> activities = issue.Activities
            .OrderBy(activity => activity.CreatedAtUtc)
            .Select(activity => activity.ToDto())
            .ToList();

        return Result<IReadOnlyList<IssueActivityDto>>.Success(activities);
    }
}
