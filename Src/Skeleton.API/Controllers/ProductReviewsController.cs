using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Skeleton.Application.Feature.Discount.Commands.CreateDiscount;
using Skeleton.Application.Feature.ProductReview.ReviewDto;

namespace Skeleton.Controllers;

/// <summary>
/// Product Reviews — View and manage product reviews.
/// </summary>
/// <remarks>
/// **Customer**: can submit and view reviews.
/// **Admin**: can delete any review.
/// **Employee**: read-only access to reviews.
/// Anonymous users can view reviews.
/// </remarks>
[ApiController]
[Route("api/[controller]")]
public class ProductReviewsController : BaseController
{
    private readonly IMediator _mediator;

    public ProductReviewsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Get all reviews for a specific product.</summary>
    [HttpGet("by-product/{productId:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByProduct(Guid productId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetReviewsByProductQuery(productId), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Submit a review for a product. **Customer only.**
    /// </summary>
    /// <remarks>
    /// Rating must be between 1 and 5.
    /// A customer can only review a product they have previously purchased.
    /// </remarks>
    [HttpPost]
    [Authorize(Roles = "Customer")]
    [EnableRateLimiting("WritePolicy")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create(
        [FromBody] CreateReviewDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateReviewCommand(dto), cancellationToken);
        return HandleResult(result, isCreated: true);
    }

    /// <summary>Delete a review. **Admin only.**</summary>
    [HttpDelete("{reviewId:guid}")]
    [Authorize(Roles = "Admin")]
    [EnableRateLimiting("WritePolicy")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid reviewId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteReviewCommand(reviewId), cancellationToken);
        return HandleResult(result);
    }
}
