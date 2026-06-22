using IssueBoard.Application.Abstractions.Messaging;
using IssueBoard.Application.Dtos;

namespace IssueBoard.Application.Issues.Comments.Add;

public sealed record AddIssueCommentCommand(
    Guid IssueId,
    string Content,
    Guid RequesterUserId) : ICommand<IssueCommentDto>;
