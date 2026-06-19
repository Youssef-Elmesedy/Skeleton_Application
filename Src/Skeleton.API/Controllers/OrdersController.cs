using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Skeleton.Application.Feature.Order.Commands.CancelOrder;
using Skeleton.Application.Feature.Order.Commands.CreateOrder;
using Skeleton.Application.Feature.Order.Commands.UpdateOrderStatus;
using Skeleton.Application.Feature.Order.OrderDto;
using Skeleton.Application.Feature.Order.Queries.GetAll;

namespace Skeleton.Controllers;

/// <summary>
/// Orders — Create and manage customer orders.
/// </summary>
/// <remarks>
/// **Customer**: create orders from their own cart, view their own orders, cancel pending orders.
/// **Employee**: view all orders, update order status.
/// **Admin**: full access including status updates.
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : BaseController
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Get all orders. **Admin + Employee.**</summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Employee")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllOrdersQuery(), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>Get orders paginated. **Admin + Employee.**</summary>
    [HttpGet("paged")]
    [Authorize(Roles = "Admin,Employee")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetPaged(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize   = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetPagedOrdersQuery(pageNumber, pageSize), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Get a specific order by ID.
    /// </summary>
    /// <remarks>
    /// **Customer**: can only view their own orders.
    /// </remarks>
    [HttpGet("{orderId:guid}")]
    [Authorize(Roles = "Admin,Employee,Customer")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid orderId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetOrderByIdQuery(orderId), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Get all orders for a specific customer.
    /// </summary>
    /// <remarks>
    /// **Customer**: can only view their own orders (must match JWT CustomerId).
    /// **Admin + Employee**: can view orders for any customer.
    /// </remarks>
    [HttpGet("by-customer/{customerId:guid}")]
    [Authorize(Roles = "Admin,Employee,Customer")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetByCustomer(Guid customerId, CancellationToken cancellationToken)
    {
        if (User.IsInRole("Customer"))
        {
            var claimId = User.FindFirst("CustomerId")?.Value;
            if (claimId is null || !Guid.TryParse(claimId, out var cId) || cId != customerId)
                return Forbid();
        }

        var result = await _mediator.Send(new GetOrdersByCustomerQuery(customerId), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Create a new order from the customer's cart. **Customer + Employee + Admin.**
    /// </summary>
    /// <remarks>
    /// The cart must be non-empty. Optionally apply a coupon code.
    /// After order creation, the cart is automatically cleared.
    /// </remarks>
    [HttpPost("{customerId:guid}")]
    [Authorize(Roles = "Admin,Employee,Customer")]
    [EnableRateLimiting("WritePolicy")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create(
        Guid customerId,
        [FromBody] CreateOrderDto dto,
        CancellationToken cancellationToken)
    {
        if (User.IsInRole("Customer"))
        {
            var claimId = User.FindFirst("CustomerId")?.Value;
            if (claimId is null || !Guid.TryParse(claimId, out var cId) || cId != customerId)
                return Forbid();
        }

        var result = await _mediator.Send(new CreateOrderCommand(customerId, dto), cancellationToken);
        return HandleResult(result, isCreated: true);
    }

    /// <summary>
    /// Update the status of an order. **Admin + Employee.**
    /// </summary>
    /// <remarks>
    /// Valid statuses: `Pending`, `Confirmed`, `Shipped`, `Delivered`, `Cancelled`, `Refunded`
    /// </remarks>
    [HttpPatch("{orderId:guid}/status")]
    [Authorize(Roles = "Admin,Employee")]
    [EnableRateLimiting("WritePolicy")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(
        Guid orderId,
        [FromBody] UpdateOrderStatusDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateOrderStatusCommand(orderId, dto), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Cancel an order. **Admin + Employee + Customer (own orders only).**
    /// </summary>
    /// <remarks>
    /// Cannot cancel orders that are already `Shipped` or `Delivered`.
    /// </remarks>
    [HttpDelete("{orderId:guid}/cancel")]
    [Authorize(Roles = "Admin,Employee,Customer")]
    [EnableRateLimiting("WritePolicy")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancel(Guid orderId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CancelOrderCommand(orderId), cancellationToken);
        return HandleResult(result);
    }
}
