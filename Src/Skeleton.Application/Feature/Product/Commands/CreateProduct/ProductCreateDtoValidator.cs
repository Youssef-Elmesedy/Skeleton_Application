using FluentValidation;

namespace Skeleton.Application.Feature.Product.Commands.CreateProduct;

public class ProductCreateDtoValidator : AbstractValidator<CreateProductCommand>
{
    public ProductCreateDtoValidator()
    {
        RuleFor(x => x.Dto.FullName)
            .NotEmpty().WithMessage("FullName is required");

        RuleFor(x => x.Dto.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero");

        RuleFor(x => x.Dto.Description)
            .NotEmpty().WithMessage("Description is required");

        RuleFor(x => x.Dto.StockQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("StockQuantity must be >= 0");

        RuleFor(x => x.Dto.Discount)
            .GreaterThanOrEqualTo(0).WithMessage("Discount must be >= 0");
    }
}
