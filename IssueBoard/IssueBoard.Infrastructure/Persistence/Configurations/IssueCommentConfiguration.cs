using IssueBoard.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IssueBoard.Infrastructure.Persistence.Configurations;

public sealed class IssueCommentConfiguration : IEntityTypeConfiguration<IssueComment>
{
    public void Configure(EntityTypeBuilder<IssueComment> builder)
    {
        builder.ToTable("IssueComments");

        builder.HasKey(comment => comment.Id);

        builder.Property(comment => comment.IssueId)
            .IsRequired();

        builder.Property(comment => comment.AuthorUserId)
            .IsRequired();

        builder.Property(comment => comment.Content)
            .HasMaxLength(4000)
            .IsRequired();

        builder.Property(comment => comment.CreatedAtUtc)
            .IsRequired();

        builder.Property(comment => comment.UpdatedAtUtc);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(comment => comment.AuthorUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
