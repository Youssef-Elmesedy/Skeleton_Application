using MediatR;
using Skeleton.Application.Behaviors;

namespace Skeleton.Application.Feature.Product.Queries.GetById;

public sealed class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Result<ProductResponseDto>>
{
    private readonly IProductService _productService;

    public GetProductByIdQueryHandler(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<Result<ProductResponseDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var product = await _productService.GetProductByIdAsync(request.ProductId, cancellationToken);

            return Result<ProductResponseDto>.Success("Get Product By_Id Sucssesfully.", product);
        }
        catch (BusinessException ex)
        {
            return ResultHelper.FromBusinessException<ProductResponseDto>(ex);
        }
        catch (Exception ex)
        {
            return Result<ProductResponseDto>.Failure("Something went wrong.", CommonErrors.Failure($"{ex.Message}"));
        }
    }
}
