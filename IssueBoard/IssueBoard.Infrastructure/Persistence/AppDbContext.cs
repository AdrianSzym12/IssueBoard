using System.Collections.Generic;
using System.Reflection.Emit;
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
