using IssueBoard.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IssueBoard.Infrastructure.Persistence.Configurations;

public sealed class IssueLabelConfiguration : IEntityTypeConfiguration<IssueLabel>
{
    public void Configure(EntityTypeBuilder<IssueLabel> builder)
    {
        builder.ToTable("IssueLabels");

        builder.HasKey(label => label.Id);

        builder.Property(label => label.ProjectId)
            .IsRequired();

        builder.Property(label => label.Name)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(label => label.ColorHex)
            .HasMaxLength(7)
            .IsRequired();

        builder.Property(label => label.CreatedAtUtc)
            .IsRequired();

        builder.Property(label => label.UpdatedAtUtc);

        builder.HasIndex(label => new { label.ProjectId, label.Name })
            .IsUnique();

        builder.HasOne<Project>()
            .WithMany()
            .HasForeignKey(label => label.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
