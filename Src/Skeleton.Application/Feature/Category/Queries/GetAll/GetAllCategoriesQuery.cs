using MediatR;
using Skeleton.Application.Feature.Category.CategoryDto;

namespace Skeleton.Application.Feature.Category.Queries.GetAll;

public record GetAllCategoriesQuery : IRequest<Result<IReadOnlyList<CategoryResponseDto>>>;
