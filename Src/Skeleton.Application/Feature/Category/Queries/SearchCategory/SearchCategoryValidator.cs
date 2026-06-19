using FluentValidation;

namespace Skeleton.Application.Feature.Category.Queries.SearchCategory;

public class SearchCategoryValidator : AbstractValidator<SearchCategoryQuery>
{
    public SearchCategoryValidator()
    {
        RuleFor(c => c.KyWord)
            .NotEmpty().WithMessage("Please Enter KyWord With Search.");
    }
}
