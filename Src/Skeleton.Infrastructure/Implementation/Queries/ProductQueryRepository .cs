using Microsoft.EntityFrameworkCore;
using Skeleton.Application.Common;
using Skeleton.Application.Feature.Product.ProductDto;
using Skeleton.Application.Interfaces.Queries;
using Skeleton.Infrastructure.Common.Extensions;
using Skeleton.Infrastructure.Common.ProjectionsExtensions;
using Skeleton.Infrastructure.Persistence;

internal class ProductQueryRepository : IProductQueryRepository
{
    private readonly AppDbContext _context;
    public ProductQueryRepository(AppDbContext context) => _context = context;

    public async Task<IReadOnlyList<ProductResponseDto>> GetAllProductsAsync(CancellationToken cancellationToken)
    {
        return await _context.Products
            .AsNoTracking()
            .Select(AsProjections.AsProductResponseDto)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ProductResponseDto>> GetByCategoryAsync(Guid categoryId, CancellationToken cancellationToken)
        => await _context.Products
            .Where(p => p.CategoryId == categoryId)
            .AsNoTracking()
            .Select(AsProjections.AsProductResponseDto)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<ProductsLowStokResponseDto>> GetLowStockAsync(CancellationToken cancellationToken)
        => await _context.Products
            .Where(p => p.StockQuantity < 2)
            .AsNoTracking()
            .Select(AsProjections.AsProdutslowStockDto)
            .ToListAsync(cancellationToken);

    public async Task<ProductResponseDto?> GetProductByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
        => await _context.Products
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(AsProjections.AsProductResponseDto)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<IReadOnlyList<ProductResponseDto>> SearchAsync(
    string? keyword,
    CancellationToken cancellationToken)
    {
        var query = _context.Products.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            keyword = keyword.Trim();

            query = query.Where(p =>
                EF.Functions.Like(p.FullName, $"%{keyword}%"));
        }

        return await query
            .OrderBy(p => p.FullName)
            .Select(AsProjections.AsProductResponseDto)
            .ToListAsync(cancellationToken);
    }

    public async Task<PagedResult<ProductResponseDto>> GetPagedAsync(
        int page, int pageSize, CancellationToken cancellationToken)
    {
        var query = _context.Products
            .AsNoTracking()
            .OrderBy(p => p.FullName);

        return await query.ToPagedResultAsync(
            page,
            pageSize,
            AsProjections.AsProductResponseDto);
    }
    public async Task<PagedResult<ProductByCategoryDto>> GetProductsGroupedByCategoryAsync(
    int page, int pageSize, CancellationToken cancellationToken)
    {
        var query = _context.Categories
            .AsNoTracking()
            .Select(c => new ProductByCategoryDto(
                c.Id,
                c.Name,
                c.Products.Select(p => new ProductResponseDto(
                    p.Id,
                    p.FullName,
                    p.Description,
                    p.Price,
                    p.Discount,
                    p.StockQuantity,
                    p.CategoryId ?? Guid.Empty,
                    c.Name,
                    p.RequiresPrescription,
                    p.CreateDate,
                    p.CreateBy.OrDefault("Not User"),
                    p.ModifiedDate,
                    p.ModifiedBy.OrDefault("Not Modifide")
                )).ToList()
            ));

        // هنا نستخدم PagedResultAsync مباشرة على الـ IQueryable
        return await query.ToPagedResultAsync(page, pageSize, x => x);
    }
}