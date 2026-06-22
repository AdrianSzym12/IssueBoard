using IssueBoard.Web.Models.Enums;

namespace IssueBoard.Web.Models.Issues;

public sealed class ChangeIssueStatusRequest
{
    public IssueStatus Status { get; set; }
}
