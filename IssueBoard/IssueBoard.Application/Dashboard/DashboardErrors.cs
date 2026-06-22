using IssueBoard.Application.Common.Errors;

namespace IssueBoard.Application.Dashboard;

public static class DashboardErrors
{
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

    public static Error NotWorkspaceMember()
    {
        return Error.Forbidden(
            "Workspaces.NotMember",
            "You are not a member of this workspace.");
    }
}
