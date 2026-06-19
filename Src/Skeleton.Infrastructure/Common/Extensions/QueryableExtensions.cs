using Microsoft.EntityFrameworkCore;
using Skeleton.Application.Common;
using System.Linq.Expressions;

namespace Skeleton.Infrastructure.Common.Extensions;

public static class QueryableExtensions
{
    // ── Paging (original — keeps existing callers working) ────────
    public static async Task<PagedResult<TDto>> ToPagedResultAsync<TEntity, TDto>(
        this IQueryable<TEntity> query,
        int pageNumber,
        int pageSize,
        Expression<Func<TEntity, TDto>> map)
    {
        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
        if (pageNumber > totalPages && totalPages > 0)
            pageNumber = totalPages;

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(map)
            .ToListAsync();

        return new PagedResult<TDto>
        {
            Page = pageNumber, PageSize = pageSize,
            TotalCount = totalCount, Items = items
        };
    }

    // ── Overload: DTO query directly (for pre-projected queries) ──
    public static async Task<PagedResult<TDto>> ToPagedResultAsync<TDto>(
        this IQueryable<TDto> query,
        int pageNumber,
        int pageSize)
    {
        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
        if (pageNumber > totalPages && totalPages > 0)
            pageNumber = totalPages;

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<TDto>
        {
            Page = pageNumber, PageSize = pageSize,
            TotalCount = totalCount, Items = items
        };
    }

    // ── Performance: AsNoTrackingWithIdentityResolution ───────────
    public static IQueryable<T> AsReadOnly<T>(this IQueryable<T> query) where T : class
        => query.AsNoTrackingWithIdentityResolution();
}
