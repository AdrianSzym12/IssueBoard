using IssueBoard.Domain.Common;

namespace IssueBoard.Domain.Entities;

public sealed class User : Entity
{
    private const int EmailMaxLength = 256;
    private const int DisplayNameMaxLength = 100;
    private const int PasswordHashMaxLength = 500;

    private User()
    {
    }

    private User(Guid id, string email, string displayName, string passwordHash)
        : base(id)
    {
        Email = NormalizeEmail(email);
        DisplayName = Guard.AgainstNullOrWhiteSpace(displayName, nameof(displayName), DisplayNameMaxLength);
        PasswordHash = Guard.AgainstNullOrWhiteSpace(passwordHash, nameof(passwordHash), PasswordHashMaxLength);
        IsActive = true;
    }

    public string Email { get; private set; } = string.Empty;

    public string DisplayName { get; private set; } = string.Empty;

    public string PasswordHash { get; private set; } = string.Empty;

    public bool IsActive { get; private set; }

    public static User Create(string email, string displayName, string passwordHash)
    {
        return new User(Guid.NewGuid(), email, displayName, passwordHash);
    }

    public void ChangeDisplayName(string displayName)
    {
        string newDisplayName = Guard.AgainstNullOrWhiteSpace(
            displayName,
            nameof(displayName),
            DisplayNameMaxLength);

        if (DisplayName == newDisplayName)
        {
            return;
        }

        DisplayName = newDisplayName;
        MarkUpdated();
    }

    public void ChangePasswordHash(string passwordHash)
    {
        PasswordHash = Guard.AgainstNullOrWhiteSpace(
            passwordHash,
            nameof(passwordHash),
            PasswordHashMaxLength);

        MarkUpdated();
    }

    public void Activate()
    {
        if (IsActive)
        {
            return;
        }

        IsActive = true;
        MarkUpdated();
    }

    public void Deactivate()
    {
        if (!IsActive)
        {
            return;
        }

        IsActive = false;
        MarkUpdated();
    }

    private static string NormalizeEmail(string email)
    {
        string normalizedEmail = Guard.AgainstNullOrWhiteSpace(email, nameof(email), EmailMaxLength)
            .ToLowerInvariant();

        if (!normalizedEmail.Contains('@', StringComparison.Ordinal))
        {
            throw new DomainException("Email address must contain '@'.");
        }

        return normalizedEmail;
    }
}
