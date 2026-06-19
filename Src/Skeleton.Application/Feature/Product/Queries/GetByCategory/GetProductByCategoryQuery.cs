using MediatR;

namespace Skeleton.Application.Feature.Product.Queries.GetByCategory;

public record GetProductByCategoryQuery(Guid categoryId) :
    IRequest<Result<IReadOnlyList<ProductResponseDto>>>;
