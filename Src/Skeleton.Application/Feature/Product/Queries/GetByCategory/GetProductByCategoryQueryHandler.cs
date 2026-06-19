using MediatR;
using Skeleton.Application.Behaviors;

namespace Skeleton.Application.Feature.Product.Queries.GetByCategory;

public sealed class GetProductByCategoryQueryHandler :
    IRequestHandler<GetProductByCategoryQuery, Result<IReadOnlyList<ProductResponseDto>>>
{
    private readonly IProductService _productService;

    public GetProductByCategoryQueryHandler(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<Result<IReadOnlyList<ProductResponseDto>>> Handle(GetProductByCategoryQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var productsByCategory = await _productService.GetByCategoryAsync(request.categoryId, cancellationToken);

            return Result<IReadOnlyList<ProductResponseDto>>.Success("Return Product with Category Sussesfully.", productsByCategory);
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
