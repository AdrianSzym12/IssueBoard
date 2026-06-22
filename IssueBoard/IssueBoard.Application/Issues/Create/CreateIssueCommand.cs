using IssueBoard.Application.Abstractions.Messaging;
using IssueBoard.Application.Dtos;
using IssueBoard.Domain.Enums;

namespace IssueBoard.Application.Issues.Create;

public sealed record CreateIssueCommand(
    Guid ProjectId,
    string Title,
    string? Description,
    IssuePriority Priority,
    Guid? AssigneeUserId,
    DateTime? DueDateUtc,
    Guid RequesterUserId) : ICommand<IssueDto>;
