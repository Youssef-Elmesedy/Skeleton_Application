namespace Skeleton.Application.Feature.Category.CategoryDto;

public record CategoryResponseDto(
    Guid Id,
    string Name,
    string Description,
    int ProductsCount,
    DateTime? CreateDate,
    string? CreateBy,
    DateTime? ModifiedDat,
    string? ModifiedBy
);