using IssueBoard.Domain.Enums;

namespace IssueBoard.Application.Dtos;

public sealed record IssuePriorityCountDto(
    IssuePriority Priority,
    int Count);
