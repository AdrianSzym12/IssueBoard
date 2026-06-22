using IssueBoard.Domain.Enums;

namespace IssueBoard.Api.Contracts.Workspaces;

public sealed record ChangeWorkspaceMemberRoleRequest(
    WorkspaceRole Role);
