using IssueBoard.Application.Abstractions.Messaging;
using IssueBoard.Application.Dtos;

namespace IssueBoard.Application.Issues.Get;

public sealed record GetIssueByIdQuery(
    Guid IssueId,
    Guid RequesterUserId) : IQuery<IssueDto>;
