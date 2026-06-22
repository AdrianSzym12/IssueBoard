namespace IssueBoard.Application.Dtos;

public sealed record WorkspaceDto(
    Guid Id,
    string Name,
    string? Description,
    int MemberCount,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);
