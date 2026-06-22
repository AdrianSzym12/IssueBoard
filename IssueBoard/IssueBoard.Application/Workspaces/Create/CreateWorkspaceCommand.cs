using IssueBoard.Application.Abstractions.Messaging;
using IssueBoard.Application.Dtos;

namespace IssueBoard.Application.Workspaces.Create;

public sealed record CreateWorkspaceCommand(
    string Name,
    string? Description,
    Guid OwnerUserId) : ICommand<WorkspaceDto>;
