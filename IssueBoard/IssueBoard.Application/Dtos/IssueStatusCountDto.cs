using IssueBoard.Domain.Enums;

namespace IssueBoard.Application.Dtos;

public sealed record IssueStatusCountDto(
    IssueStatus Status,
    int Count);
