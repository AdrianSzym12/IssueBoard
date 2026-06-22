using IssueBoard.Application.Common.Errors;

namespace IssueBoard.Application.Workspaces;

public static class WorkspaceErrors
{
    public static Error NotFound(Guid workspaceId)
    {
        return Error.NotFound(
            "Workspaces.NotFound",
            $"Workspace with id '{workspaceId}' was not found.");
    }

    public static Error UserNotFoundByEmail(string email)
    {
        return Error.NotFound(
            "Users.NotFoundByEmail",
            $"User with email '{email}' was not found.");
    }

    public static Error UserAlreadyMember(string email)
    {
        return Error.Conflict(
            "Workspaces.UserAlreadyMember",
            $"User with email '{email}' is already a member of this workspace.");
    }

    public static Error NotWorkspaceMember()
    {
        return Error.Forbidden(
            "Workspaces.NotMember",
            "You are not a member of this workspace.");
    }

    public static Error MissingPermission()
    {
        return Error.Forbidden(
            "Workspaces.MissingPermission",
            "You do not have permission to perform this action.");
    }

    public static Error MemberNotFound(Guid userId)
    {
        return Error.NotFound(
            "Workspaces.MemberNotFound",
            $"Workspace member with user id '{userId}' was not found.");
    }
}
