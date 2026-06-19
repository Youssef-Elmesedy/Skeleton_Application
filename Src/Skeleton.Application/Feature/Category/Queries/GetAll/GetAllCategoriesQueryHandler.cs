using MediatR;
using Skeleton.Application.Behaviors;
using Skeleton.Application.Feature.Category.CategoryDto;

namespace Skeleton.Application.Feature.Category.Queries.GetAll;

public sealed class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, Result<IReadOnlyList<CategoryResponseDto>>>
{
    private readonly ICategoryService _categoryService;

    public GetAllCategoriesQueryHandler(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    public async Task<Result<IReadOnlyList<CategoryResponseDto>>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var Categories = await _categoryService.GetAllCategoriesAsync(cancellationToken);

            return Result<IReadOnlyList<CategoryResponseDto>>.Success("Get All Category SucssesFully.", Categories);
        }
        catch (BusinessException ex)
        {
            return ResultHelper.FromBusinessException<IReadOnlyList<CategoryResponseDto>>(ex);
        }
        catch (Exception ex)
        {
            return Result<IReadOnlyList<CategoryResponseDto>>.Failure("Something went wrong", CommonErrors.Failure($"{ex.Message}"));
        }
    }
}
