using FluentValidation;

namespace Skeleton.Application.Feature.Product.Commands.UpdateProduct
{
    public class UpdateProductValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductValidator()
        {
            RuleFor(x => x.Dto.FullName)
                .NotEmpty().WithMessage("Full Name is required")
                .MaximumLength(200).WithMessage("Full Name must be at most 200 characters");

            RuleFor(x => x.Dto.Price)
                .GreaterThan(0).WithMessage("Price must be greater than zero");

            RuleFor(x => x.Dto.Description)
                .NotEmpty().WithMessage("Description is required");

            RuleFor(x => x.Dto.Discount)
                .GreaterThanOrEqualTo(0).WithMessage("Discount cannot be negative");

            RuleFor(x => x.Dto.StockQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("Stock quantity cannot be negative");

            // Optional boolean property
            RuleFor(x => x.Dto.RequiresPrescription)
                .NotNull().WithMessage("RequiresPrescription must be specified");
        }
    }
}
