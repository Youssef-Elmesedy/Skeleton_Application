using MediatR;
using Skeleton.Application.Feature.Category.CategoryDto;

namespace Skeleton.Application.Feature.Category.Queries.SearchCategory;

public record SearchCategoryQuery(string KyWord) : IRequest<Result<IReadOnlyList<CategoryResponseDto>>>;
