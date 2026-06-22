using IssueBoard.Web.Models.Enums;

namespace IssueBoard.Web.Models.Issues;

public sealed class SearchIssuesRequest
{
    public IssueStatus? Status { get; set; }

    public IssuePriority? Priority { get; set; }

    public Guid? AssigneeUserId { get; set; }

    public string? SearchTerm { get; set; }

    public string? SortBy { get; set; } = "number";

    public bool SortDescending { get; set; }

    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 100;
}
