using IssueBoard.Application.Abstractions.Persistence;
using IssueBoard.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace IssueBoard.Infrastructure.Persistence.Repositories;

public sealed class WorkspaceRepository : IWorkspaceRepository
{
    private readonly AppDbContext _dbContext;

    public WorkspaceRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Workspace?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Workspaces
            .Include(workspace => workspace.Members)
            .SingleOrDefaultAsync(workspace => workspace.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Workspace>> ListByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Workspaces
            .Include(workspace => workspace.Members)
            .Where(workspace => workspace.Members.Any(member => member.UserId == userId))
            .OrderBy(workspace => workspace.Name)
            .ToListAsync(cancellationToken);
    }

    public void Add(Workspace workspace)
    {
        _dbContext.Workspaces.Add(workspace);
    }
}
