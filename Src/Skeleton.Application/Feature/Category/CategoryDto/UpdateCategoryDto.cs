namespace Skeleton.Application.Feature.Category.CategoryDto;

public record UpdateCategoryDto(
    Guid Id,
    string Name,
    string Description
);
