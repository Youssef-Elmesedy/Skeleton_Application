using MediatR;
using Skeleton.Application.Behaviors;
using Skeleton.Application.Feature.Category.CategoryDto;

namespace Skeleton.Application.Feature.Category.Commands.UpdateCategory;

public sealed class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, Result<CategoryResponseDto>>
{
    private readonly ICategoryService _categoryService;

    public UpdateCategoryCommandHandler(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }
    public async Task<Result<CategoryResponseDto>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var updateCategory = await _categoryService.UpdateCategoryAsync(request.Dto, cancellationToken);

            return Result<CategoryResponseDto>.Success("Update Category Sucssesfully.", updateCategory);
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
