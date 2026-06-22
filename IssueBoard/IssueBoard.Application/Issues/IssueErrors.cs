using IssueBoard.Application.Common.Errors;

namespace IssueBoard.Application.Issues;

public static class IssueErrors
{
    public static Error NotFound(Guid issueId)
    {
        return Error.NotFound(
            "Issues.NotFound",
            $"Issue with id '{issueId}' was not found.");
    }

    public static Error ProjectNotFound(Guid projectId)
    {
        return Error.NotFound(
            "Projects.NotFound",
            $"Project with id '{projectId}' was not found.");
    }

    public static Error WorkspaceNotFound(Guid workspaceId)
    {
        return Error.NotFound(
            "Workspaces.NotFound",
            $"Workspace with id '{workspaceId}' was not found.");
    }

    public static Error ProjectArchived(Guid projectId)
    {
        return Error.Conflict(
            "Projects.Archived",
            $"Project with id '{projectId}' is archived.");
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
            "Issues.MissingPermission",
            "You do not have permission to manage issues in this workspace.");
    }

    public static Error AssigneeNotWorkspaceMember(Guid assigneeUserId)
    {
        return Error.Validation(
            "Issues.AssigneeNotWorkspaceMember",
            $"User with id '{assigneeUserId}' is not a member of this workspace.");
    }
}
