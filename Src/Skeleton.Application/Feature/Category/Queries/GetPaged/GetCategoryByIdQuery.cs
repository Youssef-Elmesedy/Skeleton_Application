using MediatR;
using Skeleton.Application.Feature.Category.CategoryDto;

namespace Skeleton.Application.Feature.Category.Queries.GetPaged;

public record GetpagedCategoriesQuery(int pageNumaber, int pageSize) : IRequest<Result<PagedResult<CategoryResponseDto>>>;
