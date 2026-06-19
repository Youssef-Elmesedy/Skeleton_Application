using Skeleton.Domain.Eunm;
using Skeleton.Domain.Exceptions;

namespace Skeleton.Domain.Entities;

public class ProductReview : BaseEntity
{
    public Guid ProductId { get; private set; }
    public Guid CustomerId { get; private set; }
    public int Rating { get; private set; }
    public string Title { get; private set; } = null!;
    public string Body { get; private set; } = null!;
    public bool IsVerifiedPurchase { get; private set; }

    public Product Product { get; private set; } = null!;
    public Customer Customer { get; private set; } = null!;

    public ProductReview(Guid productId, Guid customerId, int rating, string title, string body, bool isVerifiedPurchase = false)
    {
        if (rating < 1 || rating > 5)
            throw new BusinessException(ErrorType.Validation, "Rating must be between 1 and 5.");
        if (string.IsNullOrWhiteSpace(title))
            throw new BusinessException(ErrorType.Validation, "Review title is required.");
        if (string.IsNullOrWhiteSpace(body))
            throw new BusinessException(ErrorType.Validation, "Review body is required.");

        ProductId = productId;
        CustomerId = customerId;
        Rating = rating;
        Title = title;
        Body = body;
        IsVerifiedPurchase = isVerifiedPurchase;
    }
}
