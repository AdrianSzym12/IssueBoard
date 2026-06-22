using IssueBoard.Application.Abstractions.Persistence;
using IssueBoard.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace IssueBoard.Infrastructure.Persistence.Repositories;

public sealed class ProjectRepository : IProjectRepository
{
    private readonly AppDbContext _dbContext;

    public ProjectRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Project?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Projects
            .SingleOrDefaultAsync(project => project.Id == id, cancellationToken);
    }

    public Task<Project?> GetByWorkspaceAndKeyAsync(
        Guid workspaceId,
        string key,
        CancellationToken cancellationToken = default)
    {
        string normalizedKey = key.Trim().ToUpperInvariant();

        return _dbContext.Projects
            .SingleOrDefaultAsync(
                project => project.WorkspaceId == workspaceId && project.Key == normalizedKey,
                cancellationToken);
    }

    public async Task<IReadOnlyList<Project>> ListByWorkspaceIdAsync(
        Guid workspaceId,
        bool includeArchived = false,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Project> query = _dbContext.Projects
            .Where(project => project.WorkspaceId == workspaceId);

        if (!includeArchived)
        {
            query = query.Where(project => !project.IsArchived);
        }

        return await query
            .OrderBy(project => project.Name)
            .ToListAsync(cancellationToken);
    }

    public void Add(Project project)
    {
        _dbContext.Projects.Add(project);
    }
}
