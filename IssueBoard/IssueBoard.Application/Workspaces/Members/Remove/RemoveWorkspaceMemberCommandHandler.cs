using IssueBoard.Application.Abstractions.Messaging;
using IssueBoard.Application.Abstractions.Persistence;
using IssueBoard.Application.Common.Errors;
using IssueBoard.Domain.Entities;

namespace IssueBoard.Application.Workspaces.Members.Remove;

public sealed class RemoveWorkspaceMemberCommandHandler
    : ICommandHandler<RemoveWorkspaceMemberCommand>
{
    private readonly IWorkspaceRepository _workspaceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveWorkspaceMemberCommandHandler(
        IWorkspaceRepository workspaceRepository,
        IUnitOfWork unitOfWork)
    {
        _workspaceRepository = workspaceRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        RemoveWorkspaceMemberCommand request,
        CancellationToken cancellationToken)
    {
        Workspace? workspace = await _workspaceRepository.GetByIdAsync(
            request.WorkspaceId,
            cancellationToken);

        if (workspace is null)
        {
            return Result.Failure(
                WorkspaceErrors.NotFound(request.WorkspaceId));
        }

        bool targetMemberExists = workspace.Members
            .Any(member => member.UserId == request.UserId);

        if (!targetMemberExists)
        {
            return Result.Failure(
                WorkspaceErrors.MemberNotFound(request.UserId));
        }

        if (!WorkspaceAuthorization.CanRemoveMember(
                workspace,
                request.RequesterUserId,
                request.UserId))
        {
            return Result.Failure(
                WorkspaceErrors.MissingPermission());
        }

        workspace.RemoveMember(request.UserId);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
