using MediatR;
using Skeleton.Application.Behaviors;

namespace Skeleton.Application.Feature.Product.Queries.GetAll;

public sealed class GetAllProductQueryHandler : IRequestHandler<GetAllProductQuery, Result<IReadOnlyList<ProductResponseDto>>>
{
    private readonly IProductService _productService;

    public GetAllProductQueryHandler(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<Result<IReadOnlyList<ProductResponseDto>>> Handle(GetAllProductQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var products = await _productService.GetAllProductsAsync(cancellationToken);

            return Result<IReadOnlyList<ProductResponseDto>>.Success("Get All Product Sucssesfully", products);
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
