namespace IssueBoard.Application.Dtos;

public sealed record AuthResponseDto(
    Guid UserId,
    string Email,
    string DisplayName,
    string AccessToken,
    DateTime ExpiresAtUtc);
