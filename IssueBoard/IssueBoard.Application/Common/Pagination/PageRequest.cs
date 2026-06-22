namespace IssueBoard.Application.Common.Pagination;

public sealed record PageRequest
{
    public const int DefaultPageNumber = 1;
    public const int DefaultPageSize = 20;
    public const int MaxPageSize = 100;

    public PageRequest(int pageNumber = DefaultPageNumber, int pageSize = DefaultPageSize)
    {
        PageNumber = pageNumber < 1 ? DefaultPageNumber : pageNumber;
        PageSize = pageSize switch
        {
            < 1 => DefaultPageSize,
            > MaxPageSize => MaxPageSize,
            _ => pageSize
        };
    }

    public int PageNumber { get; }

    public int PageSize { get; }
}
