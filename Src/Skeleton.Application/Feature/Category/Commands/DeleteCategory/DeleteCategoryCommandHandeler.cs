using MediatR;
using Skeleton.Application.Behaviors;

namespace Skeleton.Application.Feature.Category.Commands.DeleteCategory;

public sealed class DeleteCategoryCommandHandeler : IRequestHandler<DeleteCategoryCommand, Result<string>>
{
    private readonly ICategoryService _categoryService;
    public DeleteCategoryCommandHandeler(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }
    public async Task<Result<string>> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _categoryService.DeleteCategoryAsync(request.categoryId, cancellationToken);

            return Result<string>.Success("Category Deleted successfully.", $"Delete Category with Id: {request.categoryId}");
        }
        catch (BusinessException ex)
        {
            return ResultHelper.FromBusinessException<string>(ex);
        }
        catch (Exception ex)
        {
            return Result<string>.Failure("Delete Category Sucssfully", CommonErrors.Failure($"{ex.Message}"));
        }
    }
}
