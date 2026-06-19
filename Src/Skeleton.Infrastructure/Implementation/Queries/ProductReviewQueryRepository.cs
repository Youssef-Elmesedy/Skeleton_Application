using Microsoft.EntityFrameworkCore;
using Skeleton.Application.Feature.ProductReview.ReviewDto;
using Skeleton.Application.Interfaces.Queries;
using Skeleton.Infrastructure.Persistence;

namespace Skeleton.Infrastructure.Implementation.Queries;

// ════════════════════════════════════════
//  ProductReview Query Repository
// ════════════════════════════════════════
internal class ProductReviewQueryRepository : IProductReviewQueryRepository
{
    private readonly AppDbContext _context;
    public ProductReviewQueryRepository(AppDbContext context) => _context = context;

    public async Task<IReadOnlyList<ReviewResponseDto>> GetReviewsByProductIdAsync(Guid productId, CancellationToken cancellationToken)
    {
        return await _context.ProductReviews
            .AsNoTracking()
            .Include(r => r.Customer)
            .Where(r => r.ProductId == productId)
            .OrderByDescending(r => r.CreateDate)
            .Select(r => new ReviewResponseDto(
                r.Id,
                r.ProductId,
                r.CustomerId,
                r.Customer != null ? r.Customer.FullName : string.Empty,
                r.Rating,
                r.Title,
                r.Body,
                r.IsVerifiedPurchase,
                r.CreateDate))
            .ToListAsync(cancellationToken);
    }
}
