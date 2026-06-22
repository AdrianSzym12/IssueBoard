using IssueBoard.Application.Dtos;
using IssueBoard.Domain.Entities;

namespace IssueBoard.Application.Issues.Comments;

internal static class IssueCommentMapper
{
    public static IssueCommentDto ToDto(this IssueComment comment)
    {
        return new IssueCommentDto(
            comment.Id,
            comment.IssueId,
            comment.AuthorUserId,
            comment.Content,
            comment.CreatedAtUtc,
            comment.UpdatedAtUtc);
    }
}
