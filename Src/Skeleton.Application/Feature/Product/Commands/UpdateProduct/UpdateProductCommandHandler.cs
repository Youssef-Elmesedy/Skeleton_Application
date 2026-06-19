using MediatR;
using Skeleton.Application.Behaviors;

namespace Skeleton.Application.Feature.Product.Commands.UpdateProduct;

public sealed class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Result<ProductResponseDto>>
{
    private readonly IProductService _productService;

    public UpdateProductCommandHandler(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<Result<ProductResponseDto>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var product = await _productService.UpdateProductAsync(request.Dto, cancellationToken);
            return Result<ProductResponseDto>.Success("Update Product Sucssesfully.", product);
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
