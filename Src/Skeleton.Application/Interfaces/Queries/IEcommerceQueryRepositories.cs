using Skeleton.Application.Feature.Payment.PaymentDto;
using Skeleton.Application.Feature.ProductReview.ReviewDto;

namespace Skeleton.Application.Interfaces.Queries;

public interface IPaymentQueryRepository
{
    Task<PaymentResponseDto?> GetPaymentByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<PaymentResponseDto>> GetPaymentsByOrderIdAsync(Guid orderId, CancellationToken cancellationToken);
}

public interface IProductReviewQueryRepository
{
    Task<IReadOnlyList<ReviewResponseDto>> GetReviewsByProductIdAsync(Guid productId, CancellationToken cancellationToken);
}
