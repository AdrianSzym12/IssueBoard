namespace IssueBoard.Api.Common.Errors;

public sealed record ApiErrorResponse(
    string Title,
    int Status,
    string Detail,
    string? TraceId = null,
    IReadOnlyDictionary<string, string[]>? Errors = null);
