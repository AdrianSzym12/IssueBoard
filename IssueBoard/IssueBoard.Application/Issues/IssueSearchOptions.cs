using IssueBoard.Domain.Enums;

namespace IssueBoard.Application.Issues;

public sealed record IssueSearchOptions(
    Guid ProjectId,
    IssueStatus? Status,
    IssuePriority? Priority,
    Guid? AssigneeUserId,
    string? SearchTerm,
    string SortBy,
    bool SortDescending,
    int PageNumber,
    int PageSize);
