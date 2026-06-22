using IssueBoard.Application.Abstractions.Persistence;
using IssueBoard.Application.Common.Pagination;
using IssueBoard.Application.Issues;
using IssueBoard.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace IssueBoard.Infrastructure.Persistence.Repositories;

public sealed class IssueRepository : IIssueRepository
{
    private readonly AppDbContext _dbContext;

    public IssueRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Issue?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Issues
            .Include(issue => issue.Comments)
            .Include(issue => issue.Activities)
            .Include(issue => issue.Labels)
            .AsSplitQuery()
            .SingleOrDefaultAsync(issue => issue.Id == id, cancellationToken);
    }

    public Task<Issue?> GetByProjectAndNumberAsync(
        Guid projectId,
        int number,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Issues
            .Include(issue => issue.Comments)
            .Include(issue => issue.Activities)
            .Include(issue => issue.Labels)
            .AsSplitQuery()
            .SingleOrDefaultAsync(
                issue => issue.ProjectId == projectId && issue.Number == number,
                cancellationToken);
    }

    public async Task<IReadOnlyList<Issue>> ListByProjectIdAsync(
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Issues
            .Include(issue => issue.Labels)
            .Where(issue => issue.ProjectId == projectId)
            .OrderBy(issue => issue.Number)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);
    }

    public async Task<PagedList<Issue>> SearchAsync(
        IssueSearchOptions options,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Issue> query = _dbContext.Issues
            .Include(issue => issue.Labels)
            .Where(issue => issue.ProjectId == options.ProjectId);

        if (options.Status.HasValue)
        {
            query = query.Where(issue => issue.Status == options.Status.Value);
        }

        if (options.Priority.HasValue)
        {
            query = query.Where(issue => issue.Priority == options.Priority.Value);
        }

        if (options.AssigneeUserId.HasValue)
        {
            query = query.Where(issue => issue.AssigneeUserId == options.AssigneeUserId.Value);
        }

        if (!string.IsNullOrWhiteSpace(options.SearchTerm))
        {
            query = query.Where(issue =>
                EF.Functions.ILike(issue.Title, $"%{options.SearchTerm}%"));
        }

        int totalCount = await query.CountAsync(cancellationToken);

        List<Issue> issues = await query
            .OrderByDescending(issue => issue.UpdatedAtUtc ?? issue.CreatedAtUtc)
            .ThenByDescending(issue => issue.Number)
            .Skip((options.PageNumber - 1) * options.PageSize)
            .Take(options.PageSize)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);

        return new PagedList<Issue>(
            issues,
            options.PageNumber,
            options.PageSize,
            totalCount);
    }

    public void Add(Issue issue)
    {
        _dbContext.Issues.Add(issue);
    }
}
