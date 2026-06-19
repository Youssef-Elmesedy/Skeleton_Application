namespace Skeleton.Application.Services.Interfaces;

public interface IProductService
{
    // Commands
    Task<ProductResponseDto> AddProductAsync(ProductCreateDto dto, CancellationToken cancellationToken);
    Task<ProductResponseDto> UpdateProductAsync(ProductUpdateDto dto, CancellationToken cancellationToken);
    Task DeleteProductAsync(Guid id, CancellationToken cancellationToken);

    // Queries
    Task<PagedResult<ProductResponseDto>> GetPagedProductAsync(
     int page, int size, CancellationToken cancellationToken);
    Task<PagedResult<ProductByCategoryDto>> GetProductsGroupedByCategoryAsync(
    int page, int size, CancellationToken cancellationToken);
    Task<ProductResponseDto> GetProductByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<ProductResponseDto>> GetAllProductsAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<ProductResponseDto>> GetByCategoryAsync(Guid categoryId, CancellationToken cancellationToken);
    Task<IReadOnlyList<ProductsLowStokResponseDto>> GetLowStockProductsAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<ProductResponseDto>> SearchProductsAsync(string keyword, CancellationToken cancellationToken);
}

