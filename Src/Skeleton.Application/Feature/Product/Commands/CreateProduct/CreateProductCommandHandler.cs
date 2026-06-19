using MediatR;
using Skeleton.Application.Behaviors;

namespace Skeleton.Application.Feature.Product.Commands.CreateProduct;

public sealed class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<ProductResponseDto>>
{
    private readonly IProductService _productService;

    public CreateProductCommandHandler(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<Result<ProductResponseDto>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var product = await _productService.AddProductAsync(request.Dto, cancellationToken);
            return Result<ProductResponseDto>.Success("Add Product Sucssesfully.", product);
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
