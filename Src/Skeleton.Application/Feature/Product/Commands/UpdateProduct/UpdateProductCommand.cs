using MediatR;
using Skeleton.Application.Common;

namespace Skeleton.Application.Feature.Product.Commands.UpdateProduct;

public record UpdateProductCommand(ProductUpdateDto Dto)
                     : IRequest<Result<ProductResponseDto>>;
