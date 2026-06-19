using MediatR;
using Skeleton.Application.Behaviors;
using Skeleton.Application.Feature.Category.CategoryDto;

namespace Skeleton.Application.Feature.Category.Queries.SearchCategory;

public sealed class SearchCategoryQueryHandler : IRequestHandler<SearchCategoryQuery, Result<IReadOnlyList<CategoryResponseDto>>>
{
    private readonly ICategoryService _categoryService;

    public SearchCategoryQueryHandler(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    public async Task<Result<IReadOnlyList<CategoryResponseDto>>> Handle(SearchCategoryQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var categories = await _categoryService.SearchCategoriesAsync(request.KyWord, cancellationToken);

            return Result<IReadOnlyList<CategoryResponseDto>>.Success("Search Category By Keyword ", categories);
        }
        catch (BusinessException bx)
        {
            return ResultHelper.FromBusinessException<IReadOnlyList<CategoryResponseDto>>(bx);
        }
        catch (Exception ex)
        {
            return Result<IReadOnlyList<CategoryResponseDto>>.Failure("Something went wrong", CommonErrors.Failure($"{ex.Message}"));
        }
    }
}
