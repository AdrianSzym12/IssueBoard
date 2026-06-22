using IssueBoard.Application.Workspaces;
using IssueBoard.Domain.Entities;
using IssueBoard.Domain.Enums;

namespace IssueBoard.Application.Issues;

internal static class IssueAuthorization
{
    public static bool CanViewIssues(Workspace workspace, Guid userId)
    {
        return WorkspaceAuthorization.IsMember(workspace, userId);
    }

    public static bool CanManageIssues(Workspace workspace, Guid userId)
    {
        WorkspaceRole? role = WorkspaceAuthorization.GetRole(workspace, userId);

        return role is WorkspaceRole.Owner or WorkspaceRole.Admin or WorkspaceRole.Member;
    }
}
