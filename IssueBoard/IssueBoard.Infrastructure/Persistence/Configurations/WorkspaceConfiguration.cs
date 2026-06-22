using IssueBoard.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IssueBoard.Infrastructure.Persistence.Configurations;

public sealed class WorkspaceConfiguration : IEntityTypeConfiguration<Workspace>
{
    public void Configure(EntityTypeBuilder<Workspace> builder)
    {
        builder.ToTable("Workspaces");

        builder.HasKey(workspace => workspace.Id);

        builder.Property(workspace => workspace.Name)
            .HasMaxLength(120)
            .IsRequired();

        builder.Property(workspace => workspace.Description)
            .HasMaxLength(500);

        builder.Property(workspace => workspace.CreatedAtUtc)
            .IsRequired();

        builder.Property(workspace => workspace.UpdatedAtUtc);

        builder.HasMany(workspace => workspace.Members)
            .WithOne()
            .HasForeignKey(member => member.WorkspaceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(workspace => workspace.Members)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
