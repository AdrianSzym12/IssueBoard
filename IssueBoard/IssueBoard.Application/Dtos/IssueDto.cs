using IssueBoard.Domain.Enums;

namespace IssueBoard.Application.Dtos;

public sealed record IssueDto(
    Guid Id,
    Guid ProjectId,
    int Number,
    string Title,
    string? Description,
    IssueStatus Status,
    IssuePriority Priority,
    Guid CreatedByUserId,
    Guid? AssigneeUserId,
    DateTime? DueDateUtc,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);
