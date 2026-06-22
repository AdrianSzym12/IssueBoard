namespace IssueBoard.Application.Dtos;

public sealed record IssueCommentDto(
    Guid Id,
    Guid IssueId,
    Guid AuthorUserId,
    string Content,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);
