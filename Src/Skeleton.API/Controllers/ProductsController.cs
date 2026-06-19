using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;

namespace Skeleton.Controllers;

/// <summary>
/// Products — Browse, search, and manage products.
/// </summary>
/// <remarks>
/// **Read operations**: accessible to everyone (including anonymous).
/// **Write operations** (Create/Update/Delete): **Admin only**.
/// </remarks>
[ApiController]
[Route("api/[controller]")]
public class ProductsController : BaseController
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all products (paginated).
    /// </summary>
    [HttpGet("paged")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaged(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize   = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetProductsPagedQuery(pageNumber, pageSize), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Get all products without pagination.
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllProductQuery(), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Get a single product by its ID.
    /// </summary>
    [HttpGet("{productId:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid productId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetProductByIdQuery(productId), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Search products by name.
    /// </summary>
    [HttpGet("search")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchByName(
        [FromQuery] string name,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new SearchProductByNameQuery(name), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Get all products in a specific category.
    /// </summary>
    [HttpGet("by-category/{categoryId:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByCategoryId(Guid categoryId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetProductByCategoryQuery(categoryId), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Get products grouped by category (paginated).
    /// </summary>
    [HttpGet("grouped-by-category")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetGroupedByCategory(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize   = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetProductGroupByCategoryQuery(pageNumber, pageSize), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Create a new product. **Admin only.**
    /// </summary>
    /// <remarks>
    /// - `Discount` here is a **price discount** on the product itself (e.g. sale price reduction), not a coupon.
    /// - Coupon discounts are managed separately under `/api/discounts`.
    /// </remarks>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [EnableRateLimiting("WritePolicy")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create(
        [FromBody] ProductCreateDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateProductCommand(dto), cancellationToken);
        return HandleResult(result, isCreated: true);
    }

    /// <summary>
    /// Update an existing product. **Admin only.**
    /// </summary>
    [HttpPut]
    [Authorize(Roles = "Admin")]
    [EnableRateLimiting("WritePolicy")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        [FromBody] ProductUpdateDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateProductCommand(dto), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Delete a product by ID. **Admin only.**
    /// </summary>
    [HttpDelete("{productId:guid}")]
    [Authorize(Roles = "Admin")]
    [EnableRateLimiting("WritePolicy")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid productId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteProductByIdCommand(productId), cancellationToken);
        return HandleResult(result);
    }
}
