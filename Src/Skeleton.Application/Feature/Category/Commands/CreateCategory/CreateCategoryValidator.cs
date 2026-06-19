using FluentValidation;

namespace Skeleton.Application.Feature.Category.Commands.CreateCategory;

public class CreateCategoryValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryValidator()
    {
        RuleFor(c => c.Dto.Name)
        .NotEmpty().WithMessage("Category Name is required");

        RuleFor(c => c.Dto.Description)
        .NotEmpty().WithMessage("Category Description is required");
    }
}