using IssueBoard.Application.Abstractions.Messaging;
using IssueBoard.Application.Dtos;
using IssueBoard.Domain.Enums;

namespace IssueBoard.Application.Issues.ChangeStatus;

public sealed record ChangeIssueStatusCommand(
    Guid IssueId,
    IssueStatus Status,
    Guid RequesterUserId) : ICommand<IssueDto>;
