using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Skeleton.Application.Feature.Discount.Commands.CreateDiscount;
using Skeleton.Application.Feature.Discount.DiscountDto;

namespace Skeleton.Controllers;

/// <summary>
/// Discounts — Manage coupon codes and promotion rules.
/// </summary>
/// <remarks>
/// **Admin**: full access — create, update, delete, view all discounts.
/// **Employee**: view discounts and validate coupon codes.
/// **Customer**: validate a coupon code before checkout only.
///
/// **Note**: This module manages **coupon-based discounts** (e.g. `SUMMER20`).
/// Product-level price reductions are managed directly on each product via `Product.Discount`.
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DiscountsController : BaseController
{
    private readonly IMediator _mediator;

    public DiscountsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Get all discount coupon codes. **Admin + Employee.**</summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Employee")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllDiscountsQuery(), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Validate a coupon code against an order total.
    /// </summary>
    /// <remarks>
    /// Available to **all authenticated users**.
    /// Returns whether the coupon is valid and the discount amount.
    /// </remarks>
    [HttpGet("validate")]
    [Authorize(Roles = "Admin,Employee,Customer")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Validate(
        [FromQuery] string code,
        [FromQuery] decimal orderAmount,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new ValidateCouponQuery(code, orderAmount), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>Create a new discount coupon. **Admin only.**</summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [EnableRateLimiting("WritePolicy")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(
        [FromBody] CreateDiscountDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateDiscountCommand(dto), cancellationToken);
        return HandleResult(result, isCreated: true);
    }

    /// <summary>Update an existing discount. **Admin only.**</summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [EnableRateLimiting("WritePolicy")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateDiscountDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateDiscountCommand(id, dto), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>Delete a discount coupon. **Admin only.**</summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [EnableRateLimiting("WritePolicy")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteDiscountCommand(id), cancellationToken);
        return HandleResult(result);
    }
}
