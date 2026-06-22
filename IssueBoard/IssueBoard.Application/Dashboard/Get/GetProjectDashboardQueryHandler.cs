using IssueBoard.Application.Abstractions.Messaging;
using IssueBoard.Application.Abstractions.Persistence;
using IssueBoard.Application.Common.Errors;
using IssueBoard.Application.Dtos;
using IssueBoard.Application.Issues;
using IssueBoard.Application.Issues.Activities;
using IssueBoard.Domain.Entities;
using IssueBoard.Domain.Enums;

namespace IssueBoard.Application.Dashboard.Get;

public sealed class GetProjectDashboardQueryHandler
    : IQueryHandler<GetProjectDashboardQuery, ProjectDashboardDto>
{
    private const int DashboardListLimit = 10;

    private readonly IProjectRepository _projectRepository;
    private readonly IWorkspaceRepository _workspaceRepository;
    private readonly IIssueRepository _issueRepository;

    public GetProjectDashboardQueryHandler(
        IProjectRepository projectRepository,
        IWorkspaceRepository workspaceRepository,
        IIssueRepository issueRepository)
    {
        _projectRepository = projectRepository;
        _workspaceRepository = workspaceRepository;
        _issueRepository = issueRepository;
    }

    public async Task<Result<ProjectDashboardDto>> Handle(
        GetProjectDashboardQuery request,
        CancellationToken cancellationToken)
    {
        Project? project = await _projectRepository.GetByIdAsync(
            request.ProjectId,
            cancellationToken);

        if (project is null)
        {
            return Result<ProjectDashboardDto>.Failure(
                DashboardErrors.ProjectNotFound(request.ProjectId));
        }

        Workspace? workspace = await _workspaceRepository.GetByIdAsync(
            project.WorkspaceId,
            cancellationToken);

        if (workspace is null)
        {
            return Result<ProjectDashboardDto>.Failure(
                DashboardErrors.WorkspaceNotFound(project.WorkspaceId));
        }

        if (!IssueAuthorization.CanViewIssues(workspace, request.RequesterUserId))
        {
            return Result<ProjectDashboardDto>.Failure(
                DashboardErrors.NotWorkspaceMember());
        }

        IReadOnlyList<Issue> issues = await _issueRepository.ListByProjectIdAsync(
            project.Id,
            cancellationToken);

        DateTime utcNow = DateTime.UtcNow;

        IReadOnlyDictionary<IssueStatus, int> statusCounts = issues
            .GroupBy(issue => issue.Status)
            .ToDictionary(group => group.Key, group => group.Count());

        IReadOnlyDictionary<IssuePriority, int> priorityCounts = issues
            .GroupBy(issue => issue.Priority)
            .ToDictionary(group => group.Key, group => group.Count());

        IReadOnlyList<IssueStatusCountDto> issuesByStatus = Enum
            .GetValues<IssueStatus>()
            .Select(status => new IssueStatusCountDto(
                status,
                statusCounts.TryGetValue(status, out int count) ? count : 0))
            .ToList();

        IReadOnlyList<IssuePriorityCountDto> issuesByPriority = Enum
            .GetValues<IssuePriority>()
            .Select(priority => new IssuePriorityCountDto(
                priority,
                priorityCounts.TryGetValue(priority, out int count) ? count : 0))
            .ToList();

        IReadOnlyList<IssueDto> overdueIssues = issues
            .Where(issue =>
                issue.DueDateUtc.HasValue &&
                issue.DueDateUtc.Value < utcNow &&
                issue.Status != IssueStatus.Done)
            .OrderBy(issue => issue.DueDateUtc)
            .Take(DashboardListLimit)
            .Select(issue => issue.ToDto())
            .ToList();

        IReadOnlyList<IssueDto> recentlyUpdatedIssues = issues
            .OrderByDescending(issue => issue.UpdatedAtUtc ?? issue.CreatedAtUtc)
            .ThenByDescending(issue => issue.Number)
            .Take(DashboardListLimit)
            .Select(issue => issue.ToDto())
            .ToList();

        IReadOnlyList<IssueActivityDto> recentActivities = issues
            .SelectMany(issue => issue.Activities)
            .OrderByDescending(activity => activity.CreatedAtUtc)
            .Take(DashboardListLimit)
            .Select(activity => activity.ToDto())
            .ToList();

        IReadOnlyList<IssueDto> myAssignedIssues = issues
            .Where(issue =>
                issue.AssigneeUserId == request.RequesterUserId &&
                issue.Status != IssueStatus.Done)
            .OrderBy(issue => issue.DueDateUtc ?? DateTime.MaxValue)
            .ThenByDescending(issue => issue.UpdatedAtUtc ?? issue.CreatedAtUtc)
            .Take(DashboardListLimit)
            .Select(issue => issue.ToDto())
            .ToList();

        int completedIssues = issues.Count(issue => issue.Status == IssueStatus.Done);

        ProjectDashboardDto dashboard = new(
            project.Id,
            project.Name,
            project.Key,
            issues.Count,
            issues.Count - completedIssues,
            completedIssues,
            issuesByStatus,
            issuesByPriority,
            overdueIssues,
            recentlyUpdatedIssues,
            recentActivities,
            myAssignedIssues);

        return Result<ProjectDashboardDto>.Success(dashboard);
    }
}
