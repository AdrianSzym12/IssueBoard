using IssueBoard.Application.Abstractions.Messaging;
using IssueBoard.Application.Dtos;

namespace IssueBoard.Application.Issues.Update;

public sealed record UpdateIssueCommand(
    Guid IssueId,
    string Title,
    string? Description,
    DateTime? DueDateUtc,
    Guid RequesterUserId) : ICommand<IssueDto>;
