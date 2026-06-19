using Skeleton.Application.Feature.Discount.DiscountDto;
using Skeleton.Application.Feature.ProductReview.ReviewDto;

namespace Skeleton.Application.Services.Interfaces;

public interface IDiscountService
{
    Task<DiscountResponseDto> CreateDiscountAsync(CreateDiscountDto dto, CancellationToken cancellationToken);
    Task<DiscountResponseDto> UpdateDiscountAsync(Guid id, UpdateDiscountDto dto, CancellationToken cancellationToken);
    Task DeleteDiscountAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<DiscountResponseDto>> GetAllDiscountsAsync(CancellationToken cancellationToken);
    Task<DiscountResponseDto> GetDiscountByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<ValidateCouponResponseDto> ValidateCouponAsync(string code, decimal orderAmount, CancellationToken cancellationToken);
}

//public interface IPaymentService
//{
//    Task<PaymentResponseDto> ProcessPaymentAsync(ProcessPaymentDto dto, CancellationToken cancellationToken);
//    Task<PaymentResponseDto> RefundPaymentAsync(Guid paymentId, CancellationToken cancellationToken);
//    Task<IReadOnlyList<PaymentResponseDto>> GetPaymentsByOrderIdAsync(Guid orderId, CancellationToken cancellationToken);
//}

public interface IProductReviewService
{
    Task<ReviewResponseDto> CreateReviewAsync(CreateReviewDto dto, CancellationToken cancellationToken);
    Task DeleteReviewAsync(Guid reviewId, CancellationToken cancellationToken);
    Task<IReadOnlyList<ReviewResponseDto>> GetReviewsByProductIdAsync(Guid productId, CancellationToken cancellationToken);
}
