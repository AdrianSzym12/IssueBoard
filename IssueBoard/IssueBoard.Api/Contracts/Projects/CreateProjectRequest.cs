namespace IssueBoard.Api.Contracts.Projects;

public sealed record CreateProjectRequest(
    string Name,
    string Key,
    string? Description);
