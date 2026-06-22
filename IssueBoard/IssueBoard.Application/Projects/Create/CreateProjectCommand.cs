using IssueBoard.Application.Abstractions.Messaging;
using IssueBoard.Application.Dtos;

namespace IssueBoard.Application.Projects.Create;

public sealed record CreateProjectCommand(
    Guid WorkspaceId,
    string Name,
    string Key,
    string? Description,
    Guid RequesterUserId) : ICommand<ProjectDto>;
