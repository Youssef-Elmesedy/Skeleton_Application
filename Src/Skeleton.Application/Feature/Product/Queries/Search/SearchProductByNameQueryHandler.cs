using MediatR;
using Skeleton.Application.Behaviors;

namespace Skeleton.Application.Feature.Product.Queries.Search;

public sealed class SearchProductByNameQueryHandler : IRequestHandler<SearchProductByNameQuery, Result<IReadOnlyList<ProductResponseDto>>>
{
    private readonly IProductService _productService;

    public SearchProductByNameQueryHandler(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<Result<IReadOnlyList<ProductResponseDto>>> Handle(SearchProductByNameQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var product = await _productService.SearchProductsAsync(request.name, cancellationToken);

            return Result<IReadOnlyList<ProductResponseDto>>.Success("Search Product with Keyword Sucssesfully.", product);
        }
        catch (BusinessException ex)
        {
            return ResultHelper.FromBusinessException<IReadOnlyList<ProductResponseDto>>(ex);
        }
        catch (Exception ex)
        {
            return Result<IReadOnlyList<ProductResponseDto>>.Failure("Something went wrong.", CommonErrors.Failure($"{ex.Message}"));
        }
    }
}
