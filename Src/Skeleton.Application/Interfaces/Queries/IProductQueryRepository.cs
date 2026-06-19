namespace Skeleton.Application.Interfaces.Queries;

public interface IProductQueryRepository
{
    Task<IReadOnlyList<ProductResponseDto>> GetAllProductsAsync(CancellationToken cancellationToken);
    Task<ProductResponseDto?> GetProductByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<ProductResponseDto>> GetByCategoryAsync(Guid categoryId, CancellationToken cancellationToken);
    Task<IReadOnlyList<ProductsLowStokResponseDto>> GetLowStockAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<ProductResponseDto>> SearchAsync(string keyword, CancellationToken cancellationToken);
    Task<PagedResult<ProductResponseDto>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken);
    Task<PagedResult<ProductByCategoryDto>> GetProductsGroupedByCategoryAsync(
    int page, int pageSize, CancellationToken cancellationToken);
}
