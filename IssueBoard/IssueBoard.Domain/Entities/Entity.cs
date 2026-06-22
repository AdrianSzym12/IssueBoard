namespace IssueBoard.Domain.Common;

public abstract class Entity
{
    protected Entity()
    {
    }

    protected Entity(Guid id)
    {
        if (id == Guid.Empty)
        {
            throw new DomainException("Entity id cannot be empty.");
        }

        Id = id;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; protected set; }

    public DateTime CreatedAtUtc { get; protected set; }

    public DateTime? UpdatedAtUtc { get; protected set; }

    protected void MarkUpdated()
    {
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
