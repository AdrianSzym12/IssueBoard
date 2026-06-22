namespace IssueBoard.Application.Dtos;

public sealed record ProjectDashboardDto(
    Guid ProjectId,
    string ProjectName,
    string ProjectKey,
    int TotalIssues,
    int OpenIssues,
    int CompletedIssues,
    IReadOnlyList<IssueStatusCountDto> IssuesByStatus,
    IReadOnlyList<IssuePriorityCountDto> IssuesByPriority,
    IReadOnlyList<IssueDto> OverdueIssues,
    IReadOnlyList<IssueDto> RecentlyUpdatedIssues,
    IReadOnlyList<IssueActivityDto> RecentActivities,
    IReadOnlyList<IssueDto> MyAssignedIssues);
