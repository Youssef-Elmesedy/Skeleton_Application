using Microsoft.EntityFrameworkCore;
using Skeleton.Application.Common;
using Skeleton.Application.Feature.Category.CategoryDto;
using Skeleton.Application.Interfaces.Queries;
using Skeleton.Infrastructure.Common.Extensions;
using Skeleton.Infrastructure.Common.ProjectionsExtensions;
using Skeleton.Infrastructure.Persistence;

internal class CategoryQueryRepository : ICategoryQueryRepository
{
    private readonly AppDbContext _context;
    public CategoryQueryRepository(AppDbContext context) => _context = context;

    public async Task<IReadOnlyList<CategoryResponseDto>> GetAllCategoriesAsync(CancellationToken cancellationToken)
    {
        return await _context.Categories
            .AsNoTracking()
            .Select(AsProjections.AsCategoryResponseDto)
            .ToListAsync(cancellationToken);
    }

    public async Task<CategoryResponseDto?> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Categories
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(AsProjections.AsCategoryResponseDto)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CategoryResponseDto>> SearchAsync(
    string? keyword,
    CancellationToken cancellationToken)
    {
        var query = _context.Categories.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            keyword = keyword.Trim();

            query = query.Where(c =>
                EF.Functions.Like(c.Name, $"%{keyword}%"));
        }

        return await query
            .OrderBy(c => c.Name)
            .Select(AsProjections.AsCategoryResponseDto)
            .ToListAsync(cancellationToken);
    }

    public async Task<PagedResult<CategoryResponseDto>> GetPagedAsync(
        int page, int pageSize, CancellationToken cancellationToken)
    {
        var query = _context.Categories
            .AsNoTracking()
            .OrderBy(c => c.Name);

        return await query.ToPagedResultAsync(
            page,
            pageSize,
            AsProjections.AsCategoryResponseDto
        );
    }
}