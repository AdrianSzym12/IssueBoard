using IssueBoard.Application.Common.Pagination;
using IssueBoard.Application.Issues;
using IssueBoard.Domain.Entities;

namespace IssueBoard.Application.Abstractions.Persistence;

public interface IIssueRepository
{
    Task<Issue?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Issue?> GetByIdForUpdateAsync(
    Guid id,
    CancellationToken cancellationToken = default);

    Task<Issue?> GetByProjectAndNumberAsync(
        Guid projectId,
        int number,
        CancellationToken cancellationToken = default);

    Task<int> GetNextIssueNumberAsync(
        Guid projectId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Issue>> ListByProjectIdAsync(
        Guid projectId,
        CancellationToken cancellationToken = default);

    Task<PagedList<Issue>> SearchAsync(
        IssueSearchOptions options,
        CancellationToken cancellationToken = default);

    void Add(Issue issue);
}
