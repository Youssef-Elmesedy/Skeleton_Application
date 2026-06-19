using MediatR;

namespace Skeleton.Application.Feature.Product.Queries.GetProductGroupByCategory;

public record GetProductGroupByCategoryQuery(int Page, int PageSize) : IRequest<Result<PagedResult<ProductByCategoryDto>>>
{
}
