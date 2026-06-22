using IssueBoard.Application.Dtos;
using IssueBoard.Domain.Entities;

namespace IssueBoard.Application.Workspaces;

internal static class WorkspaceMapper
{
    public static WorkspaceDto ToDto(this Workspace workspace)
    {
        return new WorkspaceDto(
            workspace.Id,
            workspace.Name,
            workspace.Description,
            workspace.Members.Count,
            workspace.CreatedAtUtc,
            workspace.UpdatedAtUtc);
    }

    public static WorkspaceMemberDto ToDto(this WorkspaceMember member, User? user)
    {
        return new WorkspaceMemberDto(
            member.Id,
            member.WorkspaceId,
            member.UserId,
            user?.Email,
            user?.DisplayName,
            member.Role,
            member.JoinedAtUtc);
    }

    public static IReadOnlyList<WorkspaceMemberDto> ToMemberDtos(
        this IReadOnlyCollection<WorkspaceMember> members,
        IReadOnlyCollection<User> users)
    {
        Dictionary<Guid, User> usersById = users.ToDictionary(user => user.Id);

        return members
            .OrderBy(member => member.Role)
            .ThenBy(member => member.JoinedAtUtc)
            .Select(member =>
            {
                usersById.TryGetValue(member.UserId, out User? user);

                return member.ToDto(user);
            })
            .ToList();
    }
}
