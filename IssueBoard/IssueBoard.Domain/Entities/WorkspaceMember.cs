using IssueBoard.Domain.Common;
using IssueBoard.Domain.Enums;

namespace IssueBoard.Domain.Entities;

public sealed class WorkspaceMember : Entity
{
    private WorkspaceMember()
    {
    }

    internal WorkspaceMember(Guid workspaceId, Guid userId, WorkspaceRole role)
        : base(Guid.NewGuid())
    {
        WorkspaceId = Guard.AgainstEmpty(workspaceId, nameof(workspaceId));
        UserId = Guard.AgainstEmpty(userId, nameof(userId));
        Role = Guard.AgainstInvalidEnum(role, nameof(role));
        JoinedAtUtc = DateTime.UtcNow;
    }

    public Guid WorkspaceId { get; private set; }

    public Guid UserId { get; private set; }

    public WorkspaceRole Role { get; private set; }

    public DateTime JoinedAtUtc { get; private set; }

    public void ChangeRole(WorkspaceRole role)
    {
        WorkspaceRole newRole = Guard.AgainstInvalidEnum(role, nameof(role));

        if (Role == newRole)
        {
            return;
        }

        Role = newRole;
        MarkUpdated();
    }
}
