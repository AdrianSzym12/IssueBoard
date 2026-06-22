using IssueBoard.Application.Abstractions.Messaging;
using IssueBoard.Application.Dtos;

namespace IssueBoard.Application.Issues.Activities.List;

public sealed record ListIssueActivitiesQuery(
    Guid IssueId,
    Guid RequesterUserId) : IQuery<IReadOnlyList<IssueActivityDto>>;
