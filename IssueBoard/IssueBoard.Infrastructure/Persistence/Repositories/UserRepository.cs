using IssueBoard.Application.Abstractions.Persistence;
using IssueBoard.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace IssueBoard.Infrastructure.Persistence.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly AppDbContext _dbContext;

    public UserRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Users
            .SingleOrDefaultAsync(user => user.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<User>> ListByIdsAsync(
        IReadOnlyCollection<Guid> ids,
        CancellationToken cancellationToken = default)
    {
        if (ids.Count == 0)
        {
            return Array.Empty<User>();
        }

        return await _dbContext.Users
            .Where(user => ids.Contains(user.Id))
            .ToListAsync(cancellationToken);
    }

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        string normalizedEmail = email.Trim().ToLowerInvariant();

        return _dbContext.Users
            .SingleOrDefaultAsync(user => user.Email == normalizedEmail, cancellationToken);
    }

    public Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        string normalizedEmail = email.Trim().ToLowerInvariant();

        return _dbContext.Users
            .AnyAsync(user => user.Email == normalizedEmail, cancellationToken);
    }

    public void Add(User user)
    {
        _dbContext.Users.Add(user);
    }
}
