using IssueBoard.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IssueBoard.Infrastructure.Persistence.Configurations;

public sealed class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("Projects");

        builder.HasKey(project => project.Id);

        builder.Property(project => project.WorkspaceId)
            .IsRequired();

        builder.Property(project => project.Name)
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(project => project.Key)
            .HasMaxLength(12)
            .IsRequired();

        builder.Property(project => project.Description)
            .HasMaxLength(1000);

        builder.Property(project => project.IsArchived)
            .IsRequired();

        builder.Property(project => project.CreatedAtUtc)
            .IsRequired();

        builder.Property(project => project.UpdatedAtUtc);

        builder.HasIndex(project => new { project.WorkspaceId, project.Key })
            .IsUnique();

        builder.HasOne<Workspace>()
            .WithMany()
            .HasForeignKey(project => project.WorkspaceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
