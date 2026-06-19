using MediatR;
using Skeleton.Application.Feature.Category.CategoryDto;

namespace Skeleton.Application.Feature.Category.Commands.CreateCategory;

public record CreateCategoryCommand(CreateCategoryDto Dto
) : IRequest<Result<CategoryResponseDto>>;