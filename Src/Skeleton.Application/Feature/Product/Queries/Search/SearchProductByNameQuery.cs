using MediatR;

namespace Skeleton.Application.Feature.Product.Queries.Search;

public record SearchProductByNameQuery(string name) : IRequest<Result<IReadOnlyList<ProductResponseDto>>>;

