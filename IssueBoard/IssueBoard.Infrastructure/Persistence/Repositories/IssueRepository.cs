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

    public async Task<int> GetNextIssueNumberAsync(
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        int? currentMaxNumber = await _dbContext.Issues
            .Where(issue => issue.ProjectId == projectId)
            .Select(issue => (int?)issue.Number)
            .MaxAsync(cancellationToken);

        return (currentMaxNumber ?? 0) + 1;
    }

    public async Task<IReadOnlyList<Issue>> ListByProjectIdAsync(
    Guid projectId,
    CancellationToken cancellationToken = default)
    {
        return await _dbContext.Issues
            .Include(issue => issue.Labels)
            .Include(issue => issue.Activities)
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
            string searchTerm = options.SearchTerm.Trim();

            query = query.Where(issue =>
                EF.Functions.ILike(issue.Title, $"%{searchTerm}%"));
        }

        int totalCount = await query.CountAsync(cancellationToken);

        IOrderedQueryable<Issue> orderedQuery = ApplySorting(
            query,
            options.SortBy,
            options.SortDescending);

        List<Issue> issues = await orderedQuery
            .ThenByDescending(issue => issue.Number)
            .Skip((options.PageNumber - 1) * options.PageSize)
            .Take(options.PageSize)
            .Include(issue => issue.Labels)
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

    private static IOrderedQueryable<Issue> ApplySorting(
        IQueryable<Issue> query,
        string sortBy,
        bool sortDescending)
    {
        string normalizedSortBy = sortBy.Trim().ToLowerInvariant();

        return normalizedSortBy switch
        {
            "number" => sortDescending
                ? query.OrderByDescending(issue => issue.Number)
                : query.OrderBy(issue => issue.Number),

            "title" => sortDescending
                ? query.OrderByDescending(issue => issue.Title)
                : query.OrderBy(issue => issue.Title),

            "status" => sortDescending
                ? query.OrderByDescending(issue => issue.Status)
                : query.OrderBy(issue => issue.Status),

            "priority" => sortDescending
                ? query.OrderByDescending(issue => issue.Priority)
                : query.OrderBy(issue => issue.Priority),

            "duedateutc" or "duedate" => sortDescending
                ? query.OrderByDescending(issue => issue.DueDateUtc)
                : query.OrderBy(issue => issue.DueDateUtc),

            "createdatutc" or "created" => sortDescending
                ? query.OrderByDescending(issue => issue.CreatedAtUtc)
                : query.OrderBy(issue => issue.CreatedAtUtc),

            "updatedatutc" or "updated" => sortDescending
                ? query.OrderByDescending(issue => issue.UpdatedAtUtc ?? issue.CreatedAtUtc)
                : query.OrderBy(issue => issue.UpdatedAtUtc ?? issue.CreatedAtUtc),

            _ => sortDescending
                ? query.OrderByDescending(issue => issue.UpdatedAtUtc ?? issue.CreatedAtUtc)
                : query.OrderBy(issue => issue.UpdatedAtUtc ?? issue.CreatedAtUtc)
        };
    }
}
