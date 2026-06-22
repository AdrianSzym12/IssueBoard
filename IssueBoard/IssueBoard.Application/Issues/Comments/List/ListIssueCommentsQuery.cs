using IssueBoard.Application.Abstractions.Messaging;
using IssueBoard.Application.Dtos;

namespace IssueBoard.Application.Issues.Comments.List;

public sealed record ListIssueCommentsQuery(
    Guid IssueId,
    Guid RequesterUserId) : IQuery<IReadOnlyList<IssueCommentDto>>;
