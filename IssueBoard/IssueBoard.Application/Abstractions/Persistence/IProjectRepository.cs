using IssueBoard.Domain.Entities;

namespace IssueBoard.Application.Abstractions.Persistence;

public interface IProjectRepository
{
    Task<Project?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Project?> GetByWorkspaceAndKeyAsync(
        Guid workspaceId,
        string key,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Project>> ListByWorkspaceIdAsync(
        Guid workspaceId,
        bool includeArchived = false,
        CancellationToken cancellationToken = default);

    void Add(Project project);
}
