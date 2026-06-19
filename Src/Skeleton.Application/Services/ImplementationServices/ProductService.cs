namespace Skeleton.Application.Services.ImplementationServices;

public class ProductService : IProductService
{
    private readonly IReadRepository<Product> _readRepository;
    private readonly IWriteRepository<Product> _writeRepository;
    private readonly IProductQueryRepository _queryRepository;
    private readonly ProductBusinessRules _productBusinessRules;
    private readonly IMapper _mapper;

    // Constructure Product
    public ProductService(
        IReadRepository<Product> readRepository,
        IWriteRepository<Product> writeRepository,
        IProductQueryRepository queryRepository,
        ProductBusinessRules productBusinessRules,
        IMapper mapper)
    {
        _readRepository = readRepository;
        _writeRepository = writeRepository;
        _queryRepository = queryRepository;
        _productBusinessRules = productBusinessRules;
        _mapper = mapper;
    }

    // =======================
    // Queries
    // =======================

    public async Task<PagedResult<ProductResponseDto>> GetPagedProductAsync(
        int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        if (pageNumber <= 0) pageNumber = PaginationDefaults.PageNumber;
        if (pageSize <= 0) pageSize = PaginationDefaults.PageSize;

        return await _queryRepository.GetPagedAsync(pageNumber, pageSize, cancellationToken);
    }

    public async Task<PagedResult<ProductByCategoryDto>> GetProductsGroupedByCategoryAsync(
    int page, int size, CancellationToken cancellationToken)
    {
        var pagination = await _queryRepository.GetProductsGroupedByCategoryAsync(page, size, cancellationToken);
        return pagination;
    }

    public Task<IReadOnlyList<ProductResponseDto>> GetAllProductsAsync(
        CancellationToken cancellationToken)
        => _queryRepository.GetAllProductsAsync(cancellationToken);

    public async Task<ProductResponseDto> GetProductByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var product = await _queryRepository.GetProductByIdAsync(id, cancellationToken);

        DtoBusinessRules.EnsureExists(product, id, nameof(Product));

        return product!;
    }

    public async Task<IReadOnlyList<ProductResponseDto>> GetByCategoryAsync(
        Guid categoryId,
        CancellationToken cancellationToken)
    {
        await _productBusinessRules.EnsureCategoryExists(categoryId, cancellationToken);

        return await _queryRepository.GetByCategoryAsync(categoryId, cancellationToken);
    }

    public Task<IReadOnlyList<ProductsLowStokResponseDto>> GetLowStockProductsAsync(
        CancellationToken cancellationToken)
        => _queryRepository.GetLowStockAsync(cancellationToken);

    public async Task<IReadOnlyList<ProductResponseDto>> SearchProductsAsync(
        string keyword,
        CancellationToken cancellationToken)
     => await _queryRepository.SearchAsync(keyword, cancellationToken);

    // =======================
    // Writes
    // =======================

    public async Task<ProductResponseDto> AddProductAsync(
        ProductCreateDto dto,
        CancellationToken cancellationToken)
    {
        await _productBusinessRules.EnsureUniqueAsync(p => p.FullName == dto.FullName,
            "Product name already exists.", cancellationToken);

        if (dto.CategoryId.HasValue)
            await _productBusinessRules.EnsureCategoryExists(
                dto.CategoryId.Value,
                cancellationToken);

        var product = new Product(
            dto.FullName,
            dto.Price,
            dto.Description,
            dto.CategoryId,
            dto.Discount,
            dto.StockQuantity,
            dto.RequiresPrescription
        );

        await _writeRepository.AddAsync(product);
        await _writeRepository.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ProductResponseDto>(product);
    }

    public async Task<ProductResponseDto> UpdateProductAsync(
        ProductUpdateDto dto,
        CancellationToken cancellationToken)
    {
        var product = await _readRepository.GetByIdAsync(dto.Id, cancellationToken);

        var existingProduct = _productBusinessRules.EnsureExists(product, dto.Id, nameof(Product));

        if (!string.IsNullOrWhiteSpace(dto.FullName))
            await _productBusinessRules.EnsureUniqueAsync(p => p.FullName == dto.FullName
              && p.Id != dto.Id, "Product name already exists.", cancellationToken);

        if (dto.CategoryId.HasValue)
        {
            await _productBusinessRules.EnsureCategoryExists(dto.CategoryId.Value, cancellationToken);

            existingProduct.ChangeCategory(dto.CategoryId.Value);
        }
        else
            existingProduct.RemoveCategory();

        existingProduct.Update(
            dto.FullName,
            dto.Price,
            dto.Description,
            dto.CategoryId,
            dto.Discount,
            dto.StockQuantity,
            dto.RequiresPrescription
        );

        await _writeRepository.UpdateAsync(existingProduct);
        await _writeRepository.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ProductResponseDto>(existingProduct);
    }

    public async Task DeleteProductAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var product = await _readRepository.GetByIdAsync(id, cancellationToken);

        var existingProduct = _productBusinessRules.EnsureExists(product, id, nameof(Product));

        await _writeRepository.DeleteAsync(existingProduct);

        await _writeRepository.SaveChangesAsync(cancellationToken);
    }
}