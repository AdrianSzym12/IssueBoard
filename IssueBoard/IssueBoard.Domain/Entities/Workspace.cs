using IssueBoard.Domain.Common;
using IssueBoard.Domain.Enums;

namespace IssueBoard.Domain.Entities;

public sealed class Workspace : Entity
{
    private const int NameMaxLength = 120;
    private const int DescriptionMaxLength = 500;

    private readonly List<WorkspaceMember> _members = new();

    private Workspace()
    {
    }

    private Workspace(Guid id, string name, string? description, Guid ownerUserId)
        : base(id)
    {
        Name = Guard.AgainstNullOrWhiteSpace(name, nameof(name), NameMaxLength);
        Description = Guard.OptionalText(description, nameof(description), DescriptionMaxLength);

        AddMemberInternal(ownerUserId, WorkspaceRole.Owner);
    }

    public string Name { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public IReadOnlyCollection<WorkspaceMember> Members => _members.AsReadOnly();

    public static Workspace Create(string name, string? description, Guid ownerUserId)
    {
        Guard.AgainstEmpty(ownerUserId, nameof(ownerUserId));

        return new Workspace(Guid.NewGuid(), name, description, ownerUserId);
    }

    public void Rename(string name)
    {
        string newName = Guard.AgainstNullOrWhiteSpace(name, nameof(name), NameMaxLength);

        if (Name == newName)
        {
            return;
        }

        Name = newName;
        MarkUpdated();
    }

    public void UpdateDescription(string? description)
    {
        string? newDescription = Guard.OptionalText(description, nameof(description), DescriptionMaxLength);

        if (Description == newDescription)
        {
            return;
        }

        Description = newDescription;
        MarkUpdated();
    }

    public WorkspaceMember AddMember(Guid userId, WorkspaceRole role)
    {
        WorkspaceMember member = AddMemberInternal(userId, role);
        MarkUpdated();

        return member;
    }

    public void ChangeMemberRole(Guid userId, WorkspaceRole role)
    {
        WorkspaceMember member = GetMember(userId);
        WorkspaceRole newRole = Guard.AgainstInvalidEnum(role, nameof(role));

        if (member.Role == WorkspaceRole.Owner &&
            newRole != WorkspaceRole.Owner &&
            CountOwners() == 1)
        {
            throw new DomainException("Workspace must have at least one owner.");
        }

        member.ChangeRole(newRole);
        MarkUpdated();
    }

    public void RemoveMember(Guid userId)
    {
        WorkspaceMember member = GetMember(userId);

        if (member.Role == WorkspaceRole.Owner && CountOwners() == 1)
        {
            throw new DomainException("Workspace must have at least one owner.");
        }

        _members.Remove(member);
        MarkUpdated();
    }

    public bool HasMember(Guid userId)
    {
        Guard.AgainstEmpty(userId, nameof(userId));

        return _members.Any(member => member.UserId == userId);
    }

    private WorkspaceMember AddMemberInternal(Guid userId, WorkspaceRole role)
    {
        Guard.AgainstEmpty(userId, nameof(userId));
        WorkspaceRole validRole = Guard.AgainstInvalidEnum(role, nameof(role));

        if (_members.Any(member => member.UserId == userId))
        {
            throw new DomainException("User is already a member of this workspace.");
        }

        WorkspaceMember member = new WorkspaceMember(Id, userId, validRole);
        _members.Add(member);

        return member;
    }

    private WorkspaceMember GetMember(Guid userId)
    {
        Guard.AgainstEmpty(userId, nameof(userId));

        WorkspaceMember? member = _members.SingleOrDefault(member => member.UserId == userId);

        if (member is null)
        {
            throw new DomainException("User is not a member of this workspace.");
        }

        return member;
    }

    private int CountOwners()
    {
        return _members.Count(member => member.Role == WorkspaceRole.Owner);
    }
}
