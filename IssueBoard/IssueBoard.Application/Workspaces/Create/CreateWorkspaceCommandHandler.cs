using IssueBoard.Application.Abstractions.Messaging;
using IssueBoard.Application.Abstractions.Persistence;
using IssueBoard.Application.Common.Errors;
using IssueBoard.Application.Dtos;
using IssueBoard.Domain.Entities;

namespace IssueBoard.Application.Workspaces.Create;

public sealed class CreateWorkspaceCommandHandler
    : ICommandHandler<CreateWorkspaceCommand, WorkspaceDto>
{
    private readonly IWorkspaceRepository _workspaceRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateWorkspaceCommandHandler(
        IWorkspaceRepository workspaceRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _workspaceRepository = workspaceRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<WorkspaceDto>> Handle(
        CreateWorkspaceCommand request,
        CancellationToken cancellationToken)
    {
        User? owner = await _userRepository.GetByIdAsync(
            request.OwnerUserId,
            cancellationToken);

        if (owner is null)
        {
            return Result<WorkspaceDto>.Failure(
                Error.NotFound(
                    "Users.NotFound",
                    $"User with id '{request.OwnerUserId}' was not found."));
        }

        Workspace workspace = Workspace.Create(
            request.Name,
            request.Description,
            owner.Id);

        _workspaceRepository.Add(workspace);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<WorkspaceDto>.Success(workspace.ToDto());
    }
}
