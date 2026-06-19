using MediatR;
using Skeleton.Application.Behaviors;
using Skeleton.Application.Feature.Category.CategoryDto;

namespace Skeleton.Application.Feature.Category.Queries.GetPaged;

public sealed class GetCategoryByIdQueryHandler : IRequestHandler<GetpagedCategoriesQuery, Result<PagedResult<CategoryResponseDto>>>
{
    private readonly ICategoryService _categoryService;

    public GetCategoryByIdQueryHandler(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    public async Task<Result<PagedResult<CategoryResponseDto>>> Handle(GetpagedCategoriesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var categories = await _categoryService.GetPagedCategoryAsync(request.pageNumaber, request.pageSize, cancellationToken);

            return Result<PagedResult<CategoryResponseDto>>.Success("Get All Category By Pagenation Sucssesfully.", categories);
        }
        catch (BusinessException ex)
        {
            return ResultHelper.FromBusinessException<PagedResult<CategoryResponseDto>>(ex);
        }
        catch (Exception ex)
        {
            return Result<PagedResult<CategoryResponseDto>>.Failure("Something went wrong", CommonErrors.Failure($"{ex.Message}"));
        }
    }
}
