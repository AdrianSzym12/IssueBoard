using IssueBoard.Application.Abstractions.Messaging;
using IssueBoard.Application.Dtos;

namespace IssueBoard.Application.Issues.Assign;

public sealed record AssignIssueCommand(
    Guid IssueId,
    Guid? AssigneeUserId,
    Guid RequesterUserId) : ICommand<IssueDto>;
