using IssueBoard.Application.Common.Pagination;
using IssueBoard.Domain.Enums;

namespace IssueBoard.Application.Issues;

public sealed record IssueSearchOptions
{
    public IssueSearchOptions(
        Guid projectId,
        IssueStatus? status,
        IssuePriority? priority,
        Guid? assigneeUserId,
        string? searchTerm,
        int pageNumber,
        int pageSize)
    {
        ProjectId = projectId;
        Status = status;
        Priority = priority;
        AssigneeUserId = assigneeUserId;
        SearchTerm = string.IsNullOrWhiteSpace(searchTerm) ? null : searchTerm.Trim();

        PageRequest pageRequest = new(pageNumber, pageSize);
        PageNumber = pageRequest.PageNumber;
        PageSize = pageRequest.PageSize;
    }

    public Guid ProjectId { get; }

    public IssueStatus? Status { get; }

    public IssuePriority? Priority { get; }

    public Guid? AssigneeUserId { get; }

    public string? SearchTerm { get; }

    public int PageNumber { get; }

    public int PageSize { get; }
}
