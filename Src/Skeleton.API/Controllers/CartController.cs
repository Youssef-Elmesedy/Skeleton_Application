using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Skeleton.Application.Feature.Cart.CartDto;
using Skeleton.Application.Feature.Cart.Commands.AddItem;
using Skeleton.Application.Feature.Cart.Commands.ClearCart;
using Skeleton.Application.Feature.Cart.Commands.RemoveItem;
using Skeleton.Application.Feature.Cart.Commands.UpdateItemQuantity;
using Skeleton.Application.Feature.Cart.Queries.GetCart;

namespace Skeleton.Controllers;

/// <summary>
/// Cart — Shopping cart management.
/// </summary>
/// <remarks>
/// All cart operations require authentication.
/// **Customer**: can only access their own cart (enforced via CustomerId claim).
/// **Admin + Employee**: can access any customer's cart.
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CartController : BaseController
{
    private readonly IMediator _mediator;

    public CartController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Get the cart for a specific customer.</summary>
    [HttpGet("{customerId:guid}")]
    [Authorize(Roles = "Admin,Employee,Customer")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCart(Guid customerId, CancellationToken cancellationToken)
    {
        if (User.IsInRole("Customer") && !IsOwnCustomer(customerId))
            return Forbid();

        var result = await _mediator.Send(new GetCartByCustomerIdQuery(customerId), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>Add an item to the cart.</summary>
    [HttpPost("{customerId:guid}/items")]
    [Authorize(Roles = "Admin,Employee,Customer")]
    [EnableRateLimiting("WritePolicy")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> AddItem(
        Guid customerId,
        [FromBody] AddCartItemDto dto,
        CancellationToken cancellationToken)
    {
        if (User.IsInRole("Customer") && !IsOwnCustomer(customerId))
            return Forbid();

        var result = await _mediator.Send(new AddCartItemCommand(customerId, dto), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>Update the quantity of an item in the cart.</summary>
    [HttpPut("{customerId:guid}/items")]
    [Authorize(Roles = "Admin,Employee,Customer")]
    [EnableRateLimiting("WritePolicy")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateItemQuantity(
        Guid customerId,
        [FromBody] UpdateCartItemQuantityDto dto,
        CancellationToken cancellationToken)
    {
        if (User.IsInRole("Customer") && !IsOwnCustomer(customerId))
            return Forbid();

        var result = await _mediator.Send(new UpdateCartItemQuantityCommand(customerId, dto), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>Remove a specific item from the cart.</summary>
    [HttpDelete("{customerId:guid}/items/{productId:guid}")]
    [Authorize(Roles = "Admin,Employee,Customer")]
    [EnableRateLimiting("WritePolicy")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveItem(
        Guid customerId,
        Guid productId,
        CancellationToken cancellationToken)
    {
        if (User.IsInRole("Customer") && !IsOwnCustomer(customerId))
            return Forbid();

        var result = await _mediator.Send(new RemoveCartItemCommand(customerId, productId), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>Clear all items from the cart.</summary>
    [HttpDelete("{customerId:guid}/clear")]
    [Authorize(Roles = "Admin,Employee,Customer")]
    [EnableRateLimiting("WritePolicy")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ClearCart(Guid customerId, CancellationToken cancellationToken)
    {
        if (User.IsInRole("Customer") && !IsOwnCustomer(customerId))
            return Forbid();

        var result = await _mediator.Send(new ClearCartCommand(customerId), cancellationToken);
        return HandleResult(result);
    }

    // ── Helper ──────────────────────────────────────────────────
    private bool IsOwnCustomer(Guid customerId)
    {
        var claimId = User.FindFirst("CustomerId")?.Value;
        return claimId is not null
            && Guid.TryParse(claimId, out var cId)
            && cId == customerId;
    }
}
