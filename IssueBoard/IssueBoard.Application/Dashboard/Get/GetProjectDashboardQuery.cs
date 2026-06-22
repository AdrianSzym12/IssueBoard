using IssueBoard.Application.Abstractions.Messaging;
using IssueBoard.Application.Dtos;

namespace IssueBoard.Application.Dashboard.Get;

public sealed record GetProjectDashboardQuery(
    Guid ProjectId,
    Guid RequesterUserId) : IQuery<ProjectDashboardDto>;
