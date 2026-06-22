using IssueBoard.Application.Abstractions.Messaging;
using IssueBoard.Application.Common.Pagination;
using IssueBoard.Application.Dtos;
using IssueBoard.Domain.Enums;

namespace IssueBoard.Application.Issues.List;

public sealed record ListProjectIssuesQuery(
    Guid ProjectId,
    IssueStatus? Status,
    IssuePriority? Priority,
    Guid? AssigneeUserId,
    string? SearchTerm,
    string? SortBy,
    bool SortDescending,
    int PageNumber,
    int PageSize,
    Guid RequesterUserId) : IQuery<PagedList<IssueDto>>;
