namespace Skeleton.Application.Common;

public class PagedResult<T>
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public IReadOnlyList<T?> Items { get; set; } = new List<T?>();

    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    public int? PreviousPage => Page > 1 ? Page - 1 : (int?)null;
    public int? NextPage => Page < TotalPages ? Page + 1 : (int?)null;

}
public static class PaginationDefaults
{
    public const int PageNumber = 1;
    public const int PageSize = 10;
}
