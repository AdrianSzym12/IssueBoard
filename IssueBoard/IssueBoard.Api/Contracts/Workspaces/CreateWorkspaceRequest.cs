namespace IssueBoard.Api.Contracts.Workspaces;

public sealed record CreateWorkspaceRequest(
    string Name,
    string? Description);
