using IssueBoard.Application.Abstractions.Messaging;
using IssueBoard.Application.Abstractions.Persistence;
using IssueBoard.Application.Common.Errors;
using IssueBoard.Application.Dtos;
using IssueBoard.Domain.Entities;

namespace IssueBoard.Application.Workspaces.Members.List;

public sealed class ListWorkspaceMembersQueryHandler
    : IQueryHandler<ListWorkspaceMembersQuery, IReadOnlyList<WorkspaceMemberDto>>
{
    private readonly IWorkspaceRepository _workspaceRepository;
    private readonly IUserRepository _userRepository;

    public ListWorkspaceMembersQueryHandler(
        IWorkspaceRepository workspaceRepository,
        IUserRepository userRepository)
    {
        _workspaceRepository = workspaceRepository;
        _userRepository = userRepository;
    }

    public async Task<Result<IReadOnlyList<WorkspaceMemberDto>>> Handle(
        ListWorkspaceMembersQuery request,
        CancellationToken cancellationToken)
    {
        Workspace? workspace = await _workspaceRepository.GetByIdAsync(
            request.WorkspaceId,
            cancellationToken);

        if (workspace is null)
        {
            return Result<IReadOnlyList<WorkspaceMemberDto>>.Failure(
                WorkspaceErrors.NotFound(request.WorkspaceId));
        }

        if (!WorkspaceAuthorization.IsMember(workspace, request.RequesterUserId))
        {
            return Result<IReadOnlyList<WorkspaceMemberDto>>.Failure(
                WorkspaceErrors.NotWorkspaceMember());
        }

        Guid[] userIds = workspace.Members
            .Select(member => member.UserId)
            .Distinct()
            .ToArray();

        IReadOnlyList<User> users = await _userRepository.ListByIdsAsync(
            userIds,
            cancellationToken);

        IReadOnlyList<WorkspaceMemberDto> response = workspace.Members.ToMemberDtos(users);

        return Result<IReadOnlyList<WorkspaceMemberDto>>.Success(response);
    }
}
