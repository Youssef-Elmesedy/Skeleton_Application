using MediatR;

namespace Skeleton.Application.Feature.Category.Commands.DeleteCategory;

public record DeleteCategoryCommand(Guid categoryId) : IRequest<Result<string>>;

