using IssueBoard.Domain.Enums;

namespace IssueBoard.Api.Contracts.Issues;

public sealed record ChangeIssuePriorityRequest(
    IssuePriority Priority);
