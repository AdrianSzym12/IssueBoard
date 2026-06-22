namespace IssueBoard.Api.Contracts.Issues;

public sealed record UpdateIssueRequest(
    string Title,
    string? Description,
    DateTime? DueDateUtc);
