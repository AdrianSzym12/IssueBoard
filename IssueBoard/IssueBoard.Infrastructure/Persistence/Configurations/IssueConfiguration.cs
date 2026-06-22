using IssueBoard.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IssueBoard.Infrastructure.Persistence.Configurations;

public sealed class IssueConfiguration : IEntityTypeConfiguration<Issue>
{
    public void Configure(EntityTypeBuilder<Issue> builder)
    {
        builder.ToTable("Issues");

        builder.HasKey(issue => issue.Id);

        builder.Property(issue => issue.ProjectId)
            .IsRequired();

        builder.Property(issue => issue.Number)
            .IsRequired();

        builder.Property(issue => issue.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(issue => issue.Description)
            .HasMaxLength(4000);

        builder.Property(issue => issue.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(issue => issue.Priority)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(issue => issue.CreatedByUserId)
            .IsRequired();

        builder.Property(issue => issue.AssigneeUserId);

        builder.Property(issue => issue.DueDateUtc);

        builder.Property(issue => issue.CreatedAtUtc)
            .IsRequired();

        builder.Property(issue => issue.UpdatedAtUtc);

        builder.HasIndex(issue => new { issue.ProjectId, issue.Number })
            .IsUnique();

        builder.HasIndex(issue => issue.Status);

        builder.HasIndex(issue => issue.Priority);

        builder.HasIndex(issue => issue.AssigneeUserId);

        builder.HasOne<Project>()
            .WithMany()
            .HasForeignKey(issue => issue.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(issue => issue.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(issue => issue.AssigneeUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(issue => issue.Comments)
            .WithOne()
            .HasForeignKey(comment => comment.IssueId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(issue => issue.Comments)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(issue => issue.Activities)
            .WithOne()
            .HasForeignKey(activity => activity.IssueId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(issue => issue.Activities)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(issue => issue.Labels)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "IssueIssueLabels",
                right => right
                    .HasOne<IssueLabel>()
                    .WithMany()
                    .HasForeignKey("IssueLabelId")
                    .OnDelete(DeleteBehavior.Cascade),
                left => left
                    .HasOne<Issue>()
                    .WithMany()
                    .HasForeignKey("IssueId")
                    .OnDelete(DeleteBehavior.Cascade),
                join =>
                {
                    join.ToTable("IssueIssueLabels");

                    join.HasKey("IssueId", "IssueLabelId");

                    join.IndexerProperty<Guid>("IssueId")
                        .HasColumnName("IssueId");

                    join.IndexerProperty<Guid>("IssueLabelId")
                        .HasColumnName("IssueLabelId");
                });

        builder.Navigation(issue => issue.Labels)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
