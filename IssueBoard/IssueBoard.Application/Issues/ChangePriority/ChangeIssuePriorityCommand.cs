using IssueBoard.Application.Abstractions.Messaging;
using IssueBoard.Application.Dtos;
using IssueBoard.Domain.Enums;

namespace IssueBoard.Application.Issues.ChangePriority;

public sealed record ChangeIssuePriorityCommand(
    Guid IssueId,
    IssuePriority Priority,
    Guid RequesterUserId) : ICommand<IssueDto>;
