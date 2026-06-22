namespace IssueBoard.Api.Contracts.Issues;

public sealed record AssignIssueRequest(
    Guid? AssigneeUserId);
