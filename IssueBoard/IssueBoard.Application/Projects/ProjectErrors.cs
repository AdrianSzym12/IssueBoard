using IssueBoard.Application.Common.Errors;

namespace IssueBoard.Application.Projects;

public static class ProjectErrors
{
    public static Error NotFound(Guid projectId)
    {
        return Error.NotFound(
            "Projects.NotFound",
            $"Project with id '{projectId}' was not found.");
    }

    public static Error KeyAlreadyExists(string key)
    {
        return Error.Conflict(
            "Projects.KeyAlreadyExists",
            $"Project with key '{key}' already exists in this workspace.");
    }

    public static Error WorkspaceNotFound(Guid workspaceId)
    {
        return Error.NotFound(
            "Workspaces.NotFound",
            $"Workspace with id '{workspaceId}' was not found.");
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
            "Projects.MissingPermission",
            "You do not have permission to manage projects in this workspace.");
    }
}
