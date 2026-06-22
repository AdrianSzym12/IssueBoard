using IssueBoard.Domain.Entities;
using IssueBoard.Domain.Enums;

namespace IssueBoard.Application.Workspaces;

internal static class WorkspaceAuthorization
{
    public static bool IsMember(Workspace workspace, Guid userId)
    {
        return workspace.Members.Any(member => member.UserId == userId);
    }

    public static bool CanManageMembers(Workspace workspace, Guid userId)
    {
        WorkspaceRole? role = GetRole(workspace, userId);

        return role is WorkspaceRole.Owner or WorkspaceRole.Admin;
    }

    public static bool CanAddRole(Workspace workspace, Guid requesterUserId, WorkspaceRole roleToAdd)
    {
        WorkspaceRole? requesterRole = GetRole(workspace, requesterUserId);

        if (requesterRole == WorkspaceRole.Owner)
        {
            return true;
        }

        return requesterRole == WorkspaceRole.Admin && roleToAdd is WorkspaceRole.Member or WorkspaceRole.Viewer;
    }

    public static bool CanChangeMemberRole(Workspace workspace, Guid requesterUserId)
    {
        return GetRole(workspace, requesterUserId) == WorkspaceRole.Owner;
    }

    public static bool CanRemoveMember(Workspace workspace, Guid requesterUserId, Guid targetUserId)
    {
        WorkspaceRole? requesterRole = GetRole(workspace, requesterUserId);
        WorkspaceRole? targetRole = GetRole(workspace, targetUserId);

        if (requesterRole is null || targetRole is null)
        {
            return false;
        }

        if (requesterRole == WorkspaceRole.Owner)
        {
            return true;
        }

        return requesterRole == WorkspaceRole.Admin &&
               targetRole is WorkspaceRole.Member or WorkspaceRole.Viewer;
    }

    public static WorkspaceRole? GetRole(Workspace workspace, Guid userId)
    {
        return workspace.Members
            .SingleOrDefault(member => member.UserId == userId)
            ?.Role;
    }
}
