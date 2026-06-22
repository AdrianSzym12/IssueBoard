using IssueBoard.Application.Abstractions.Messaging;

namespace IssueBoard.Application.Workspaces.Members.Remove;

public sealed record RemoveWorkspaceMemberCommand(
    Guid WorkspaceId,
    Guid UserId,
    Guid RequesterUserId) : ICommand;
