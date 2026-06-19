using MediatR;

namespace Skeleton.Application.Feature.Product.Queries.GetProductGroupByCategory;

public sealed class GetProductsGroupedByCategoryHandler : IRequestHandler<GetProductGroupByCategoryQuery, Result<PagedResult<ProductByCategoryDto>>>
{
    private readonly IProductService _productService;

    public GetProductsGroupedByCategoryHandler(IProductService productService)
        => _productService = productService;

    public async Task<Result<PagedResult<ProductByCategoryDto>>> Handle(GetProductGroupByCategoryQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _productService.GetProductsGroupedByCategoryAsync(request.Page, request.PageSize, cancellationToken);

            return Result<PagedResult<ProductByCategoryDto>>.Success("Return All Products with Category_Id Sucssesfully.", result);
        }
        catch (Exception ex)
        {
            return Result<PagedResult<ProductByCategoryDto>>.Failure("Something went wrong.", CommonErrors.Failure($"{ex.Message}"));
        }
    }
}
