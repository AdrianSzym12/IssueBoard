using IssueBoard.Domain.Entities;

namespace IssueBoard.Application.Abstractions.Persistence;

public interface IWorkspaceRepository
{
    Task<Workspace?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Workspace>> ListByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    void Add(Workspace workspace);
}
