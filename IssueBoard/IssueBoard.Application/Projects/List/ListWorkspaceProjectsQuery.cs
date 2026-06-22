using IssueBoard.Application.Abstractions.Messaging;
using IssueBoard.Application.Dtos;

namespace IssueBoard.Application.Projects.List;

public sealed record ListWorkspaceProjectsQuery(
    Guid WorkspaceId,
    Guid RequesterUserId,
    bool IncludeArchived) : IQuery<IReadOnlyList<ProjectDto>>;
