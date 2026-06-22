using IssueBoard.Application.Abstractions.Messaging;
using IssueBoard.Application.Dtos;
using IssueBoard.Domain.Enums;

namespace IssueBoard.Application.Workspaces.Members.ChangeRole;

public sealed record ChangeWorkspaceMemberRoleCommand(
    Guid WorkspaceId,
    Guid UserId,
    WorkspaceRole Role,
    Guid RequesterUserId) : ICommand<WorkspaceMemberDto>;
