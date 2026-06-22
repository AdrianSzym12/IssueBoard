using IssueBoard.Domain.Common;

namespace IssueBoard.Domain.Entities;

public sealed class IssueComment : Entity
{
    private const int ContentMaxLength = 4000;

    private IssueComment()
    {
    }

    private IssueComment(Guid issueId, Guid authorUserId, string content)
        : base(Guid.NewGuid())
    {
        IssueId = Guard.AgainstEmpty(issueId, nameof(issueId));
        AuthorUserId = Guard.AgainstEmpty(authorUserId, nameof(authorUserId));
        Content = Guard.AgainstNullOrWhiteSpace(content, nameof(content), ContentMaxLength);
    }

    public Guid IssueId { get; private set; }

    public Guid AuthorUserId { get; private set; }

    public string Content { get; private set; } = string.Empty;

    public static IssueComment Create(Guid issueId, Guid authorUserId, string content)
    {
        return new IssueComment(issueId, authorUserId, content);
    }

    public void Edit(string content)
    {
        string newContent = Guard.AgainstNullOrWhiteSpace(content, nameof(content), ContentMaxLength);

        if (Content == newContent)
        {
            return;
        }

        Content = newContent;
        MarkUpdated();
    }
}
