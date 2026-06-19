using MediatR;
using Skeleton.Application.Common;

namespace Skeleton.Application.Feature.Product.Queries.GetById;

public record GetProductByIdQuery(Guid ProductId) : IRequest<Result<ProductResponseDto>>;
