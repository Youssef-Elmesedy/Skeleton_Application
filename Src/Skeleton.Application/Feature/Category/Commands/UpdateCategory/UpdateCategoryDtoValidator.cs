using FluentValidation;

namespace Skeleton.Application.Feature.Category.Commands.UpdateCategory;

public class UpdateCategoryDtoValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryDtoValidator()
    {
        RuleFor(x => x.Dto.Id).NotEmpty().WithMessage("Id is required.");

        RuleFor(x => x.Dto.Name).NotEmpty().WithMessage("Name is required.");

        RuleFor(x => x.Dto.Description).NotEmpty().WithMessage("Description is required.");
    }
}
