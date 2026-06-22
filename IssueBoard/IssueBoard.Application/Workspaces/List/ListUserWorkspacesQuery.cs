using IssueBoard.Application.Abstractions.Messaging;
using IssueBoard.Application.Dtos;

namespace IssueBoard.Application.Workspaces.List;

public sealed record ListUserWorkspacesQuery(Guid UserId)
    : IQuery<IReadOnlyList<WorkspaceDto>>;
