using System.Text.RegularExpressions;
using IssueBoard.Domain.Common;

namespace IssueBoard.Domain.Entities;

public sealed partial class IssueLabel : Entity
{
    private const int NameMaxLength = 50;
    private const int ColorHexMaxLength = 7;

    private IssueLabel()
    {
    }

    private IssueLabel(Guid id, Guid projectId, string name, string colorHex)
        : base(id)
    {
        ProjectId = Guard.AgainstEmpty(projectId, nameof(projectId));
        Name = Guard.AgainstNullOrWhiteSpace(name, nameof(name), NameMaxLength);
        ColorHex = NormalizeColor(colorHex);
    }

    public Guid ProjectId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string ColorHex { get; private set; } = string.Empty;

    public static IssueLabel Create(Guid projectId, string name, string colorHex)
    {
        return new IssueLabel(Guid.NewGuid(), projectId, name, colorHex);
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

    public void ChangeColor(string colorHex)
    {
        string newColorHex = NormalizeColor(colorHex);

        if (ColorHex == newColorHex)
        {
            return;
        }

        ColorHex = newColorHex;
        MarkUpdated();
    }

    private static string NormalizeColor(string colorHex)
    {
        string normalizedColor = Guard.AgainstNullOrWhiteSpace(
            colorHex,
            nameof(colorHex),
            ColorHexMaxLength);

        if (!ColorHexRegex().IsMatch(normalizedColor))
        {
            throw new DomainException("Color must be a valid hex color, for example #4F46E5.");
        }

        return normalizedColor.ToUpperInvariant();
    }

    [GeneratedRegex("^#[0-9A-Fa-f]{6}$")]
    private static partial Regex ColorHexRegex();
}
