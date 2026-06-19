using MediatR;
using Skeleton.Application.Behaviors;
using Skeleton.Application.Feature.Discount.DiscountDto;
using Skeleton.Application.Feature.Payment.PaymentDto;
using Skeleton.Application.Feature.ProductReview.ReviewDto;

// ════════════════════════════════════════════════════
//  DISCOUNT COMMANDS
// ════════════════════════════════════════════════════
namespace Skeleton.Application.Feature.Discount.Commands.CreateDiscount;

public record CreateDiscountCommand(CreateDiscountDto Dto) : IRequest<Result<DiscountResponseDto>>;

public sealed class CreateDiscountCommandHandler : IRequestHandler<CreateDiscountCommand, Result<DiscountResponseDto>>
{
    private readonly IDiscountService _discountService;
    public CreateDiscountCommandHandler(IDiscountService discountService) => _discountService = discountService;

    public async Task<Result<DiscountResponseDto>> Handle(CreateDiscountCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var discount = await _discountService.CreateDiscountAsync(request.Dto, cancellationToken);
            return Result<DiscountResponseDto>.Success("Discount created successfully.", discount);
        }
        catch (BusinessException ex) { return ResultHelper.FromBusinessException<DiscountResponseDto>(ex); }
    }
}

public class CreateDiscountValidator : AbstractValidator<CreateDiscountCommand>
{
    public CreateDiscountValidator()
    {
        RuleFor(x => x.Dto.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Dto.Description).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Dto.Value).GreaterThan(0);
        RuleFor(x => x.Dto.StartDate).LessThan(x => x.Dto.EndDate).WithMessage("Start date must be before end date.");
    }
}

// ──────────────────────────────────────────────────────────────────

public record UpdateDiscountCommand(Guid Id, UpdateDiscountDto Dto) : IRequest<Result<DiscountResponseDto>>;

public sealed class UpdateDiscountCommandHandler : IRequestHandler<UpdateDiscountCommand, Result<DiscountResponseDto>>
{
    private readonly IDiscountService _discountService;
    public UpdateDiscountCommandHandler(IDiscountService discountService) => _discountService = discountService;

    public async Task<Result<DiscountResponseDto>> Handle(UpdateDiscountCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var discount = await _discountService.UpdateDiscountAsync(request.Id, request.Dto, cancellationToken);
            return Result<DiscountResponseDto>.Success("Discount updated successfully.", discount);
        }
        catch (BusinessException ex) { return ResultHelper.FromBusinessException<DiscountResponseDto>(ex); }
    }
}

// ──────────────────────────────────────────────────────────────────

public record DeleteDiscountCommand(Guid Id) : IRequest<Result<bool>>;

public sealed class DeleteDiscountCommandHandler : IRequestHandler<DeleteDiscountCommand, Result<bool>>
{
    private readonly IDiscountService _discountService;
    public DeleteDiscountCommandHandler(IDiscountService discountService) => _discountService = discountService;

    public async Task<Result<bool>> Handle(DeleteDiscountCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _discountService.DeleteDiscountAsync(request.Id, cancellationToken);
            return Result<bool>.Success("Discount deleted successfully.", true);
        }
        catch (BusinessException ex) { return ResultHelper.FromBusinessException<bool>(ex); }
    }
}

// ════════════════════════════════════════════════════
//  DISCOUNT QUERIES
// ════════════════════════════════════════════════════

public record GetAllDiscountsQuery : IRequest<Result<IReadOnlyList<DiscountResponseDto>>>;

public sealed class GetAllDiscountsQueryHandler : IRequestHandler<GetAllDiscountsQuery, Result<IReadOnlyList<DiscountResponseDto>>>
{
    private readonly IDiscountService _discountService;
    public GetAllDiscountsQueryHandler(IDiscountService discountService) => _discountService = discountService;

    public async Task<Result<IReadOnlyList<DiscountResponseDto>>> Handle(GetAllDiscountsQuery request, CancellationToken cancellationToken)
    {
        var discounts = await _discountService.GetAllDiscountsAsync(cancellationToken);
        return Result<IReadOnlyList<DiscountResponseDto>>.Success("Discounts retrieved successfully.", discounts);
    }
}

// ──────────────────────────────────────────────────────────────────

public record ValidateCouponQuery(string Code, decimal OrderAmount) : IRequest<Result<ValidateCouponResponseDto>>;

public sealed class ValidateCouponQueryHandler : IRequestHandler<ValidateCouponQuery, Result<ValidateCouponResponseDto>>
{
    private readonly IDiscountService _discountService;
    public ValidateCouponQueryHandler(IDiscountService discountService) => _discountService = discountService;

    public async Task<Result<ValidateCouponResponseDto>> Handle(ValidateCouponQuery request, CancellationToken cancellationToken)
    {
        var result = await _discountService.ValidateCouponAsync(request.Code, request.OrderAmount, cancellationToken);
        return Result<ValidateCouponResponseDto>.Success("Coupon validated.", result);
    }
}

// ════════════════════════════════════════════════════
//  PAYMENT COMMANDS
// ════════════════════════════════════════════════════

public record ProcessPaymentCommand(ProcessPaymentDto Dto) : IRequest<Result<PaymentResponseDto>>;

public sealed class ProcessPaymentCommandHandler : IRequestHandler<ProcessPaymentCommand, Result<PaymentResponseDto>>
{
    private readonly IPaymentService _paymentService;
    public ProcessPaymentCommandHandler(IPaymentService paymentService) => _paymentService = paymentService;

    public async Task<Result<PaymentResponseDto>> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var payment = await _paymentService.ProcessPaymentAsync(request.Dto, cancellationToken);
            return Result<PaymentResponseDto>.Success("Payment processed successfully.", payment);
        }
        catch (BusinessException ex) { return ResultHelper.FromBusinessException<PaymentResponseDto>(ex); }
    }
}

public class ProcessPaymentValidator : AbstractValidator<ProcessPaymentCommand>
{
    public ProcessPaymentValidator()
    {
        RuleFor(x => x.Dto.OrderId).NotEmpty();
        RuleFor(x => x.Dto.Method).IsInEnum();
    }
}

// ──────────────────────────────────────────────────────────────────

public record RefundPaymentCommand(Guid PaymentId) : IRequest<Result<PaymentResponseDto>>;

public sealed class RefundPaymentCommandHandler : IRequestHandler<RefundPaymentCommand, Result<PaymentResponseDto>>
{
    private readonly IPaymentService _paymentService;
    public RefundPaymentCommandHandler(IPaymentService paymentService) => _paymentService = paymentService;

    public async Task<Result<PaymentResponseDto>> Handle(RefundPaymentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var payment = await _paymentService.RefundPaymentAsync(request.PaymentId, cancellationToken);
            return Result<PaymentResponseDto>.Success("Payment refunded successfully.", payment);
        }
        catch (BusinessException ex) { return ResultHelper.FromBusinessException<PaymentResponseDto>(ex); }
    }
}

// ════════════════════════════════════════════════════
//  PAYMENT QUERIES
// ════════════════════════════════════════════════════

public record GetPaymentsByOrderQuery(Guid OrderId) : IRequest<Result<IReadOnlyList<PaymentResponseDto>>>;

public sealed class GetPaymentsByOrderQueryHandler : IRequestHandler<GetPaymentsByOrderQuery, Result<IReadOnlyList<PaymentResponseDto>>>
{
    private readonly IPaymentService _paymentService;
    public GetPaymentsByOrderQueryHandler(IPaymentService paymentService) => _paymentService = paymentService;

    public async Task<Result<IReadOnlyList<PaymentResponseDto>>> Handle(GetPaymentsByOrderQuery request, CancellationToken cancellationToken)
    {
        var payments = await _paymentService.GetPaymentsByOrderIdAsync(request.OrderId, cancellationToken);
        return Result<IReadOnlyList<PaymentResponseDto>>.Success("Payments retrieved successfully.", payments);
    }
}

// ════════════════════════════════════════════════════
//  PRODUCT REVIEW COMMANDS
// ════════════════════════════════════════════════════

public record CreateReviewCommand(CreateReviewDto Dto) : IRequest<Result<ReviewResponseDto>>;

public sealed class CreateReviewCommandHandler : IRequestHandler<CreateReviewCommand, Result<ReviewResponseDto>>
{
    private readonly IProductReviewService _reviewService;
    public CreateReviewCommandHandler(IProductReviewService reviewService) => _reviewService = reviewService;

    public async Task<Result<ReviewResponseDto>> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var review = await _reviewService.CreateReviewAsync(request.Dto, cancellationToken);
            return Result<ReviewResponseDto>.Success("Review created successfully.", review);
        }
        catch (BusinessException ex) { return ResultHelper.FromBusinessException<ReviewResponseDto>(ex); }
    }
}

public class CreateReviewValidator : AbstractValidator<CreateReviewCommand>
{
    public CreateReviewValidator()
    {
        RuleFor(x => x.Dto.ProductId).NotEmpty();
        RuleFor(x => x.Dto.CustomerId).NotEmpty();
        RuleFor(x => x.Dto.Rating).InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5.");
        RuleFor(x => x.Dto.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Dto.Body).NotEmpty().MaximumLength(2000);
    }
}

// ──────────────────────────────────────────────────────────────────

public record DeleteReviewCommand(Guid ReviewId) : IRequest<Result<bool>>;

public sealed class DeleteReviewCommandHandler : IRequestHandler<DeleteReviewCommand, Result<bool>>
{
    private readonly IProductReviewService _reviewService;
    public DeleteReviewCommandHandler(IProductReviewService reviewService) => _reviewService = reviewService;

    public async Task<Result<bool>> Handle(DeleteReviewCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _reviewService.DeleteReviewAsync(request.ReviewId, cancellationToken);
            return Result<bool>.Success("Review deleted successfully.", true);
        }
        catch (BusinessException ex) { return ResultHelper.FromBusinessException<bool>(ex); }
    }
}

// ════════════════════════════════════════════════════
//  PRODUCT REVIEW QUERIES
// ════════════════════════════════════════════════════

public record GetReviewsByProductQuery(Guid ProductId) : IRequest<Result<IReadOnlyList<ReviewResponseDto>>>;

public sealed class GetReviewsByProductQueryHandler : IRequestHandler<GetReviewsByProductQuery, Result<IReadOnlyList<ReviewResponseDto>>>
{
    private readonly IProductReviewService _reviewService;
    public GetReviewsByProductQueryHandler(IProductReviewService reviewService) => _reviewService = reviewService;

    public async Task<Result<IReadOnlyList<ReviewResponseDto>>> Handle(GetReviewsByProductQuery request, CancellationToken cancellationToken)
    {
        var reviews = await _reviewService.GetReviewsByProductIdAsync(request.ProductId, cancellationToken);
        return Result<IReadOnlyList<ReviewResponseDto>>.Success("Reviews retrieved successfully.", reviews);
    }
}
