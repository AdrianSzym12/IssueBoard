using IssueBoard.Application.Abstractions.Messaging;
using IssueBoard.Application.Abstractions.Persistence;
using IssueBoard.Application.Common.Errors;
using IssueBoard.Application.Dtos;
using IssueBoard.Domain.Entities;

namespace IssueBoard.Application.Workspaces.List;

public sealed class ListUserWorkspacesQueryHandler
    : IQueryHandler<ListUserWorkspacesQuery, IReadOnlyList<WorkspaceDto>>
{
    private readonly IWorkspaceRepository _workspaceRepository;

    public ListUserWorkspacesQueryHandler(IWorkspaceRepository workspaceRepository)
    {
        _workspaceRepository = workspaceRepository;
    }

    public async Task<Result<IReadOnlyList<WorkspaceDto>>> Handle(
        ListUserWorkspacesQuery request,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<Workspace> workspaces = await _workspaceRepository.ListByUserIdAsync(
            request.UserId,
            cancellationToken);

        IReadOnlyList<WorkspaceDto> response = workspaces
            .Select(workspace => workspace.ToDto())
            .ToList();

        return Result<IReadOnlyList<WorkspaceDto>>.Success(response);
    }
}
