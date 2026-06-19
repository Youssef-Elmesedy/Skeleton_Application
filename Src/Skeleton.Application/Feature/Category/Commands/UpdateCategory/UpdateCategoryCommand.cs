using MediatR;
using Skeleton.Application.Feature.Category.CategoryDto;

namespace Skeleton.Application.Feature.Category.Commands.UpdateCategory;

public record UpdateCategoryCommand(UpdateCategoryDto Dto) : IRequest<Result<CategoryResponseDto>>;
