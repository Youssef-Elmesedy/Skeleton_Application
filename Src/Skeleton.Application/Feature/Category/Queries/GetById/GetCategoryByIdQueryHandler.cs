using MediatR;
using Skeleton.Application.Behaviors;
using Skeleton.Application.Feature.Category.CategoryDto;

namespace Skeleton.Application.Feature.Category.Queries.GetById;

public sealed class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, Result<CategoryResponseDto>>
{
    private readonly ICategoryService _categoryService;

    public GetCategoryByIdQueryHandler(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    public async Task<Result<CategoryResponseDto>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var category = await _categoryService.GetCategoryByIdAsync(request.id, cancellationToken);

            return Result<CategoryResponseDto>.Success("Get Category By Id Sucssesfully.", category!);
        }
        catch (BusinessException ex)
        {
            return ResultHelper.FromBusinessException<CategoryResponseDto>(ex);
        }
        catch (Exception ex)
        {
            return Result<CategoryResponseDto>.Failure("Something went wrong", CommonErrors.Failure($"{ex.Message}"));
        }
    }
}
