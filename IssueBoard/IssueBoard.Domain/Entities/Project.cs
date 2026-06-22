using System.Text.RegularExpressions;
using IssueBoard.Domain.Common;

namespace IssueBoard.Domain.Entities;

public sealed partial class Project : Entity
{
    private const int NameMaxLength = 120;
    private const int KeyMaxLength = 12;
    private const int DescriptionMaxLength = 1000;

    private Project()
    {
    }

    private Project(Guid id, Guid workspaceId, string name, string key, string? description)
        : base(id)
    {
        WorkspaceId = Guard.AgainstEmpty(workspaceId, nameof(workspaceId));
        Name = Guard.AgainstNullOrWhiteSpace(name, nameof(name), NameMaxLength);
        Key = NormalizeKey(key);
        Description = Guard.OptionalText(description, nameof(description), DescriptionMaxLength);
        IsArchived = false;
    }

    public Guid WorkspaceId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Key { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public bool IsArchived { get; private set; }

    public static Project Create(Guid workspaceId, string name, string key, string? description)
    {
        return new Project(Guid.NewGuid(), workspaceId, name, key, description);
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

    public void ChangeKey(string key)
    {
        string newKey = NormalizeKey(key);

        if (Key == newKey)
        {
            return;
        }

        Key = newKey;
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

    public void Archive()
    {
        if (IsArchived)
        {
            return;
        }

        IsArchived = true;
        MarkUpdated();
    }

    public void Restore()
    {
        if (!IsArchived)
        {
            return;
        }

        IsArchived = false;
        MarkUpdated();
    }

    private static string NormalizeKey(string key)
    {
        string normalizedKey = Guard.AgainstNullOrWhiteSpace(key, nameof(key), KeyMaxLength)
            .ToUpperInvariant();

        if (!ProjectKeyRegex().IsMatch(normalizedKey))
        {
            throw new DomainException("Project key must start with a letter and contain only uppercase letters, numbers or hyphens.");
        }

        return normalizedKey;
    }

    [GeneratedRegex("^[A-Z][A-Z0-9-]{1,11}$")]
    private static partial Regex ProjectKeyRegex();
}
