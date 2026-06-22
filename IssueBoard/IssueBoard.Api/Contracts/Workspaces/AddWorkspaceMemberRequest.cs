using IssueBoard.Domain.Enums;

namespace IssueBoard.Api.Contracts.Workspaces;

public sealed record AddWorkspaceMemberRequest(
    string Email,
    WorkspaceRole Role);
