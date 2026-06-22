using IssueBoard.Domain.Enums;

namespace IssueBoard.Api.Contracts.Issues;

public sealed record CreateIssueRequest(
    string Title,
    string? Description,
    IssuePriority Priority,
    Guid? AssigneeUserId,
    DateTime? DueDateUtc);
