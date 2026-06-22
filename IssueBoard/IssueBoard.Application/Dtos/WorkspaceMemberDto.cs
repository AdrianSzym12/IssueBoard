using IssueBoard.Domain.Enums;

namespace IssueBoard.Application.Dtos;

public sealed record WorkspaceMemberDto(
    Guid Id,
    Guid WorkspaceId,
    Guid UserId,
    string? Email,
    string? DisplayName,
    WorkspaceRole Role,
    DateTime JoinedAtUtc);
