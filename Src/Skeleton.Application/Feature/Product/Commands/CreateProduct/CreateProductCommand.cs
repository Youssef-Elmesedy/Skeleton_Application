using MediatR;
using Skeleton.Application.Common;

namespace Skeleton.Application.Feature.Product.Commands.CreateProduct;

public record CreateProductCommand(ProductCreateDto Dto)
    : IRequest<Result<ProductResponseDto>>;
