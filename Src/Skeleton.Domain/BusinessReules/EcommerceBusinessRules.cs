using Skeleton.Domain.BusinessRules;
using Skeleton.Domain.Entities;
using Skeleton.Domain.Eunm;
using Skeleton.Domain.Exceptions;
using Skeleton.Domain.Interfaces.InterfacesRepository;

namespace Skeleton.Domain.BusinessReules;

public class DiscountBusinessRules : EntityBaseBusinessRules<Discount>
{
    public DiscountBusinessRules(IReadRepository<Discount> discountRepository)
        : base(discountRepository)
    {
    }

    public async Task EnsureCodeIsUnique(string code, CancellationToken cancellationToken)
    {
        await EnsureUniqueAsync(
            d => d.Code == code.ToUpper().Trim(),
            "A discount with this coupon code already exists.",
            cancellationToken);
    }

    public async Task EnsureCodeIsUniqueOnUpdate(string code, Guid id, CancellationToken cancellationToken)
    {
        await EnsureUniqueAsync(
            d => d.Code == code.ToUpper().Trim() && d.Id != id,
            "A discount with this coupon code already exists.",
            cancellationToken);
    }
}

public class PaymentBusinessRules : EntityBaseBusinessRules<Payment>
{
    public PaymentBusinessRules(IReadRepository<Payment> paymentRepository)
        : base(paymentRepository)
    {
    }
}

public class ProductReviewBusinessRules : EntityBaseBusinessRules<ProductReview>
{
    public ProductReviewBusinessRules(IReadRepository<ProductReview> reviewRepository)
        : base(reviewRepository)
    {
    }

    public async Task EnsureCustomerHasNotReviewed(Guid productId, Guid customerId, CancellationToken cancellationToken)
    {
        await EnsureUniqueAsync(
            r => r.ProductId == productId && r.CustomerId == customerId,
            "You have already reviewed this product.",
            cancellationToken);
    }
}
