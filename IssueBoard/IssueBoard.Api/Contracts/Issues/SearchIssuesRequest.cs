using IssueBoard.Domain.Enums;

namespace IssueBoard.Api.Contracts.Issues;

public sealed class SearchIssuesRequest
{
    public IssueStatus? Status { get; init; }

    public IssuePriority? Priority { get; init; }

    public Guid? AssigneeUserId { get; init; }

    public string? SearchTerm { get; init; }

    public string? SortBy { get; init; } = "updatedAtUtc";

    public bool SortDescending { get; init; } = true;

    public int PageNumber { get; init; } = 1;

    public int PageSize { get; init; } = 20;
}
