using MediatR;
using Skeleton.Application.Behaviors;
using Skeleton.Application.Feature.Category.CategoryDto;

namespace Skeleton.Application.Feature.Category.Commands.CreateCategory;

public sealed class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Result<CategoryResponseDto>>
{
    private readonly ICategoryService _categoryService;

    public CreateCategoryCommandHandler(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    public async Task<Result<CategoryResponseDto>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var category = await _categoryService.AddCategoryAsync(request.Dto, cancellationToken);

            return Result<CategoryResponseDto>.Success("Add Category Sucssesfully", category);
        }
        catch (BusinessException ex)
        {
            return ResultHelper.FromBusinessException<CategoryResponseDto>(ex);
        }
        catch (Exception ex)
        {
            return Result<CategoryResponseDto>.Failure("Something went wrong",
                CommonErrors.Failure($"{ex.Message}"));
        }
    }
}
