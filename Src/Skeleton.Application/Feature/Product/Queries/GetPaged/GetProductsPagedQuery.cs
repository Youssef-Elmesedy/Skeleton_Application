using MediatR;

namespace Skeleton.Application.Feature.Product.Queries.GetPaged;

public record GetProductsPagedQuery(int page, int size) : IRequest<Result<PagedResult<ProductResponseDto>>>;

