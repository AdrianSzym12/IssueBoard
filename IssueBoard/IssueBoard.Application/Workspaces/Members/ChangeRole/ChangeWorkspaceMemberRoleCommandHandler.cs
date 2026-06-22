using IssueBoard.Application.Abstractions.Messaging;
using IssueBoard.Application.Abstractions.Persistence;
using IssueBoard.Application.Common.Errors;
using IssueBoard.Application.Dtos;
using IssueBoard.Domain.Entities;

namespace IssueBoard.Application.Workspaces.Members.ChangeRole;

public sealed class ChangeWorkspaceMemberRoleCommandHandler
    : ICommandHandler<ChangeWorkspaceMemberRoleCommand, WorkspaceMemberDto>
{
    private readonly IWorkspaceRepository _workspaceRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ChangeWorkspaceMemberRoleCommandHandler(
        IWorkspaceRepository workspaceRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _workspaceRepository = workspaceRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<WorkspaceMemberDto>> Handle(
        ChangeWorkspaceMemberRoleCommand request,
        CancellationToken cancellationToken)
    {
        Workspace? workspace = await _workspaceRepository.GetByIdAsync(
            request.WorkspaceId,
            cancellationToken);

        if (workspace is null)
        {
            return Result<WorkspaceMemberDto>.Failure(
                WorkspaceErrors.NotFound(request.WorkspaceId));
        }

        if (!WorkspaceAuthorization.CanChangeMemberRole(workspace, request.RequesterUserId))
        {
            return Result<WorkspaceMemberDto>.Failure(
                WorkspaceErrors.MissingPermission());
        }

        WorkspaceMember? member = workspace.Members
            .SingleOrDefault(workspaceMember => workspaceMember.UserId == request.UserId);

        if (member is null)
        {
            return Result<WorkspaceMemberDto>.Failure(
                WorkspaceErrors.MemberNotFound(request.UserId));
        }

        workspace.ChangeMemberRole(request.UserId, request.Role);

        User? user = await _userRepository.GetByIdAsync(
            request.UserId,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<WorkspaceMemberDto>.Success(member.ToDto(user));
    }
}
