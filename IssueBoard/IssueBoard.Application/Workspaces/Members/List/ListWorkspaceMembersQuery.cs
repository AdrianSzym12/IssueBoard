using IssueBoard.Application.Abstractions.Messaging;
using IssueBoard.Application.Dtos;

namespace IssueBoard.Application.Workspaces.Members.List;

public sealed record ListWorkspaceMembersQuery(
    Guid WorkspaceId,
    Guid RequesterUserId) : IQuery<IReadOnlyList<WorkspaceMemberDto>>;
