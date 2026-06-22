using IssueBoard.Application.Abstractions.Messaging;
using IssueBoard.Application.Dtos;
using IssueBoard.Domain.Enums;

namespace IssueBoard.Application.Workspaces.Members.Add;

public sealed record AddWorkspaceMemberCommand(
    Guid WorkspaceId,
    string Email,
    WorkspaceRole Role,
    Guid RequesterUserId) : ICommand<WorkspaceMemberDto>;
