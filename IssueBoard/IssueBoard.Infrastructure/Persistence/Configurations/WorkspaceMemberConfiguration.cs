using IssueBoard.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IssueBoard.Infrastructure.Persistence.Configurations;

public sealed class WorkspaceMemberConfiguration : IEntityTypeConfiguration<WorkspaceMember>
{
    public void Configure(EntityTypeBuilder<WorkspaceMember> builder)
    {
        builder.ToTable("WorkspaceMembers");

        builder.HasKey(member => member.Id);

        builder.Property(member => member.WorkspaceId)
            .IsRequired();

        builder.Property(member => member.UserId)
            .IsRequired();

        builder.Property(member => member.Role)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(member => member.JoinedAtUtc)
            .IsRequired();

        builder.Property(member => member.CreatedAtUtc)
            .IsRequired();

        builder.Property(member => member.UpdatedAtUtc);

        builder.HasIndex(member => new { member.WorkspaceId, member.UserId })
            .IsUnique();

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(member => member.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
