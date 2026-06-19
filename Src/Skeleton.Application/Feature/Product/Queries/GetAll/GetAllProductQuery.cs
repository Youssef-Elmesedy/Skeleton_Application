using MediatR;

namespace Skeleton.Application.Feature.Product.Queries.GetAll;

public record GetAllProductQuery : IRequest<Result<IReadOnlyList<ProductResponseDto>>>;

