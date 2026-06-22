using IssueBoard.Application.Abstractions.Persistence;
using IssueBoard.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace IssueBoard.Infrastructure.Persistence;

public sealed class AppDbContext : DbContext, IUnitOfWork
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<Workspace> Workspaces => Set<Workspace>();

    public DbSet<WorkspaceMember> WorkspaceMembers => Set<WorkspaceMember>();

    public DbSet<Project> Projects => Set<Project>();

    public DbSet<Issue> Issues => Set<Issue>();

    public DbSet<IssueComment> IssueComments => Set<IssueComment>();

    public DbSet<IssueActivity> IssueActivities => Set<IssueActivity>();

    public DbSet<IssueLabel> IssueLabels => Set<IssueLabel>();

    public override async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await MarkNewIssueActivitiesAsAddedAsync(cancellationToken);
        await MarkNewIssueCommentsAsAddedAsync(cancellationToken);

        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }

    private async Task MarkNewIssueActivitiesAsAddedAsync(
        CancellationToken cancellationToken)
    {
        foreach (var entry in ChangeTracker.Entries<IssueActivity>()
                     .Where(entry => entry.State == EntityState.Modified)
                     .ToList())
        {
            bool exists = await IssueActivities
                .AsNoTracking()
                .AnyAsync(activity => activity.Id == entry.Entity.Id, cancellationToken);

            if (!exists)
            {
                entry.State = EntityState.Added;
            }
        }
    }

    private async Task MarkNewIssueCommentsAsAddedAsync(
        CancellationToken cancellationToken)
    {
        foreach (var entry in ChangeTracker.Entries<IssueComment>()
                     .Where(entry => entry.State == EntityState.Modified)
                     .ToList())
        {
            bool exists = await IssueComments
                .AsNoTracking()
                .AnyAsync(comment => comment.Id == entry.Entity.Id, cancellationToken);

            if (!exists)
            {
                entry.State = EntityState.Added;
            }
        }
    }
}
