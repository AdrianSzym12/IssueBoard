using IssueBoard.Application.Abstractions.Messaging;
using IssueBoard.Application.Abstractions.Persistence;
using IssueBoard.Application.Common.Errors;
using IssueBoard.Application.Dtos;
using IssueBoard.Domain.Entities;

namespace IssueBoard.Application.Workspaces.Members.Add;

public sealed class AddWorkspaceMemberCommandHandler
    : ICommandHandler<AddWorkspaceMemberCommand, WorkspaceMemberDto>
{
    private readonly IWorkspaceRepository _workspaceRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddWorkspaceMemberCommandHandler(
        IWorkspaceRepository workspaceRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _workspaceRepository = workspaceRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<WorkspaceMemberDto>> Handle(
        AddWorkspaceMemberCommand request,
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

        if (!WorkspaceAuthorization.CanAddRole(workspace, request.RequesterUserId, request.Role))
        {
            return Result<WorkspaceMemberDto>.Failure(
                WorkspaceErrors.MissingPermission());
        }

        User? user = await _userRepository.GetByEmailAsync(
            request.Email,
            cancellationToken);

        if (user is null)
        {
            return Result<WorkspaceMemberDto>.Failure(
                WorkspaceErrors.UserNotFoundByEmail(request.Email));
        }

        if (workspace.HasMember(user.Id))
        {
            return Result<WorkspaceMemberDto>.Failure(
                WorkspaceErrors.UserAlreadyMember(request.Email));
        }

        WorkspaceMember member = workspace.AddMember(user.Id, request.Role);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<WorkspaceMemberDto>.Success(member.ToDto(user));
    }
}
