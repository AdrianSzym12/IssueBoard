namespace IssueBoard.Application.Dtos;

public sealed record IssueActivityDto(
    Guid Id,
    Guid IssueId,
    Guid ActorUserId,
    string Action,
    string? OldValue,
    string? NewValue,
    DateTime CreatedAtUtc);
