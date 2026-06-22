namespace IssueBoard.Domain.Common;

public static class Guard
{
    public static Guid AgainstEmpty(Guid value, string parameterName)
    {
        if (value == Guid.Empty)
        {
            throw new DomainException($"{parameterName} cannot be empty.");
        }

        return value;
    }

    public static string AgainstNullOrWhiteSpace(string? value, string parameterName, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException($"{parameterName} cannot be empty.");
        }

        string trimmedValue = value.Trim();

        if (trimmedValue.Length > maxLength)
        {
            throw new DomainException($"{parameterName} cannot be longer than {maxLength} characters.");
        }

        return trimmedValue;
    }

    public static string? OptionalText(string? value, string parameterName, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        string trimmedValue = value.Trim();

        if (trimmedValue.Length > maxLength)
        {
            throw new DomainException($"{parameterName} cannot be longer than {maxLength} characters.");
        }

        return trimmedValue;
    }

    public static TEnum AgainstInvalidEnum<TEnum>(TEnum value, string parameterName)
        where TEnum : struct, Enum
    {
        if (!Enum.IsDefined(value))
        {
            throw new DomainException($"{parameterName} has invalid value.");
        }

        return value;
    }
}
