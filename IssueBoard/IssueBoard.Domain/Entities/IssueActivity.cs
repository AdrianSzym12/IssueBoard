using IssueBoard.Domain.Common;

namespace IssueBoard.Domain.Entities;

public sealed class IssueActivity : Entity
{
    private const int ActionMaxLength = 100;
    private const int ValueMaxLength = 500;

    private IssueActivity()
    {
    }

    private IssueActivity(
        Guid issueId,
        Guid actorUserId,
        string action,
        string? oldValue,
        string? newValue)
        : base(Guid.NewGuid())
    {
        IssueId = Guard.AgainstEmpty(issueId, nameof(issueId));
        ActorUserId = Guard.AgainstEmpty(actorUserId, nameof(actorUserId));
        Action = Guard.AgainstNullOrWhiteSpace(action, nameof(action), ActionMaxLength);
        OldValue = Guard.OptionalText(oldValue, nameof(oldValue), ValueMaxLength);
        NewValue = Guard.OptionalText(newValue, nameof(newValue), ValueMaxLength);
    }

    public Guid IssueId { get; private set; }

    public Guid ActorUserId { get; private set; }

    public string Action { get; private set; } = string.Empty;

    public string? OldValue { get; private set; }

    public string? NewValue { get; private set; }

    public static IssueActivity Create(
        Guid issueId,
        Guid actorUserId,
        string action,
        string? oldValue,
        string? newValue)
    {
        return new IssueActivity(issueId, actorUserId, action, oldValue, newValue);
    }
}
