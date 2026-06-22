using System.ComponentModel.DataAnnotations;
using IssueBoard.Web.Models.Enums;

namespace IssueBoard.Web.Models.Issues;

public sealed class CreateIssueRequest
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(4000)]
    public string? Description { get; set; }

    public IssuePriority Priority { get; set; } = IssuePriority.Medium;

    public Guid? AssigneeUserId { get; set; }

    public DateTime? DueDateUtc { get; set; }
}
