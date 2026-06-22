using IssueBoard.Domain.Enums;

namespace IssueBoard.Api.Contracts.Issues;

public sealed record ChangeIssueStatusRequest(
    IssueStatus Status);
