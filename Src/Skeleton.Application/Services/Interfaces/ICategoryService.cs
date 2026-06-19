using Skeleton.Application.Feature.Category.CategoryDto;

namespace Skeleton.Application.Services.Interfaces;

public interface ICategoryService
{
    // Commands
    Task<CategoryResponseDto> AddCategoryAsync(CreateCategoryDto dto, CancellationToken cancellationToken);
    Task<CategoryResponseDto> UpdateCategoryAsync(UpdateCategoryDto dto, CancellationToken cancellationToken);
    Task DeleteCategoryAsync(Guid id, CancellationToken cancellationToken);

    // Queries
    Task<PagedResult<CategoryResponseDto>> GetPagedCategoryAsync(
     int page, int size, CancellationToken cancellationToken);
    Task<IReadOnlyList<CategoryResponseDto>> GetAllCategoriesAsync(CancellationToken cancellationToken);
    Task<CategoryResponseDto?> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<CategoryResponseDto>> SearchCategoriesAsync(string keyword, CancellationToken cancellationToken);
}
