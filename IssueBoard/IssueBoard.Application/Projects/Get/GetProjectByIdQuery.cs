using IssueBoard.Application.Abstractions.Messaging;
using IssueBoard.Application.Dtos;

namespace IssueBoard.Application.Projects.Get;

public sealed record GetProjectByIdQuery(
    Guid ProjectId,
    Guid RequesterUserId) : IQuery<ProjectDto>;
