using MediatR;
using Skeleton.Application.Behaviors;

namespace Skeleton.Application.Feature.Product.Commands.DeleteProduct;

public sealed class DeleteProductByIdCommandHandeler : IRequestHandler<DeleteProductByIdCommand, Result<string>>
{
    private readonly IProductService _productService;

    public DeleteProductByIdCommandHandeler(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<Result<string>> Handle(DeleteProductByIdCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _productService.DeleteProductAsync(request.Id, cancellationToken);

            return Result<string>.Success("Product Deleted Successfully.", $"Delete Product with Id: {request.Id}");
        }
        catch (BusinessException ex)
        {
            return ResultHelper.FromBusinessException<string>(ex);
        }
        catch (Exception ex)
        {
            return Result<string>.Failure("Something went wrong.", CommonErrors.Failure($"{ex.Message}"));
        }
    }
}
