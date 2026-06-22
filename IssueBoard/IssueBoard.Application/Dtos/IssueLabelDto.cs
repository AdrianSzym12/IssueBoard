namespace IssueBoard.Application.Dtos;

public sealed record IssueLabelDto(
    Guid Id,
    Guid ProjectId,
    string Name,
    string ColorHex,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);
