namespace Skeleton.Application.Feature.ProductReview.ReviewDto;

public record ReviewResponseDto(
    Guid Id,
    Guid ProductId,
    Guid CustomerId,
    string CustomerFullName,
    int Rating,
    string Title,
    string Body,
    bool IsVerifiedPurchase,
    DateTime? CreateDate
);

public record CreateReviewDto(
    Guid ProductId,
    Guid CustomerId,
    int Rating,
    string Title,
    string Body
);
