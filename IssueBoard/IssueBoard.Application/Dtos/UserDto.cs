namespace IssueBoard.Application.Dtos;

public sealed record UserDto(
    Guid Id,
    string Email,
    string DisplayName,
    bool IsActive);
