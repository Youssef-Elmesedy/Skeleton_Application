using MediatR;

namespace Skeleton.Application.Feature.Product.Queries.GetPaged;

public sealed class GetProductsPagedQueryHandler : IRequestHandler<GetProductsPagedQuery, Result<PagedResult<ProductResponseDto>>>
{
    private readonly IProductService _productService;

    public GetProductsPagedQueryHandler(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<Result<PagedResult<ProductResponseDto>>> Handle(GetProductsPagedQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var products = await _productService.GetPagedProductAsync(request.page, request.size, cancellationToken);

            return Result<PagedResult<ProductResponseDto>>.Success("Get Product By Pagenation Sucssesfully.", products);
        }
        catch (Exception ex)
        {
            return Result<PagedResult<ProductResponseDto>>.Failure("Something went wrong.", CommonErrors.Failure($"{ex.Message}"));
        }
    }
}
