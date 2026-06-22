namespace IssueBoard.Application.Common.Errors;

public sealed record Error(string Code, string Message)
{
    public static readonly Error None = new(string.Empty, string.Empty);

    public static Error NotFound(string code, string message)
    {
        return new Error(code, message);
    }

    public static Error Validation(string code, string message)
    {
        return new Error(code, message);
    }

    public static Error Conflict(string code, string message)
    {
        return new Error(code, message);
    }

    public static Error Failure(string code, string message)
    {
        return new Error(code, message);
    }
}
