using FluentValidation;

namespace Skeleton.Application.Feature.Product.Queries.Search;

public class SearchProductByNameValidator : AbstractValidator<SearchProductByNameQuery>
{
    public SearchProductByNameValidator()
    {
        RuleFor(p => p.name)
              .NotEmpty()
              .Must(name => !string.IsNullOrWhiteSpace(name))
              .WithMessage("Product name cannot be empty or whitespace.");
    }
}
