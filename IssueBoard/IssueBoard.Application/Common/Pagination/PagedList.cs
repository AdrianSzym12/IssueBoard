namespace IssueBoard.Application.Common.Pagination;

public sealed class PagedList<TItem>
{
    public PagedList(
        IReadOnlyList<TItem> items,
        int pageNumber,
        int pageSize,
        int totalCount)
    {
        Items = items;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount;
        TotalPages = totalCount == 0
            ? 0
            : (int)Math.Ceiling(totalCount / (double)pageSize);
    }

    public IReadOnlyList<TItem> Items { get; }

    public int PageNumber { get; }

    public int PageSize { get; }

    public int TotalCount { get; }

    public int TotalPages { get; }

    public bool HasPreviousPage => PageNumber > 1;

    public bool HasNextPage => PageNumber < TotalPages;

    public static PagedList<TItem> Empty(int pageNumber, int pageSize)
    {
        return new PagedList<TItem>(
            Array.Empty<TItem>(),
            pageNumber,
            pageSize,
            totalCount: 0);
    }
}
