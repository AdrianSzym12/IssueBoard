using IssueBoard.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IssueBoard.Infrastructure.Persistence.Configurations;

public sealed class IssueActivityConfiguration : IEntityTypeConfiguration<IssueActivity>
{
    public void Configure(EntityTypeBuilder<IssueActivity> builder)
    {
        builder.ToTable("IssueActivities");

        builder.HasKey(activity => activity.Id);

        builder.Property(activity => activity.IssueId)
            .IsRequired();

        builder.Property(activity => activity.ActorUserId)
            .IsRequired();

        builder.Property(activity => activity.Action)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(activity => activity.OldValue)
            .HasMaxLength(500);

        builder.Property(activity => activity.NewValue)
            .HasMaxLength(500);

        builder.Property(activity => activity.CreatedAtUtc)
            .IsRequired();

        builder.Property(activity => activity.UpdatedAtUtc);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(activity => activity.ActorUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
