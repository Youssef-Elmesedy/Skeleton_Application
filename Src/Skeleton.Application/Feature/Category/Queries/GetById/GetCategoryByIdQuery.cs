using MediatR;
using Skeleton.Application.Feature.Category.CategoryDto;

namespace Skeleton.Application.Feature.Category.Queries.GetById;

public record GetCategoryByIdQuery(Guid id) : IRequest<Result<CategoryResponseDto>>;
