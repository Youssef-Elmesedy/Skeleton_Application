using Skeleton.Application.Feature.Category.CategoryDto;

namespace Skeleton.Application.Interfaces.Queries;

public interface ICategoryQueryRepository
{
    Task<IReadOnlyList<CategoryResponseDto>> GetAllCategoriesAsync(CancellationToken cancellationToken);
    Task<CategoryResponseDto?> GetCategoryByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<CategoryResponseDto>> SearchAsync(string keyword, CancellationToken cancellationToken);
    Task<PagedResult<CategoryResponseDto>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken);
}
