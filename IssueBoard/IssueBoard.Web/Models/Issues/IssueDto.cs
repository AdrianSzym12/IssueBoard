using IssueBoard.Web.Models.Enums;

namespace IssueBoard.Web.Models.Issues;

public sealed class IssueDto
{
    public Guid Id { get; set; }

    public Guid ProjectId { get; set; }

    public int Number { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public IssueStatus Status { get; set; }

    public IssuePriority Priority { get; set; }

    public Guid CreatedByUserId { get; set; }

    public Guid? AssigneeUserId { get; set; }

    public DateTime? DueDateUtc { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? UpdatedAtUtc { get; set; }
}
