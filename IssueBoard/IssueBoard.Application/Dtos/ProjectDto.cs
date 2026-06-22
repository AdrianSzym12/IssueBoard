namespace IssueBoard.Application.Dtos;

public sealed record ProjectDto(
    Guid Id,
    Guid WorkspaceId,
    string Name,
    string Key,
    string? Description,
    bool IsArchived,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);
