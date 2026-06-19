using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Skeleton.Application.Feature.Customer.Commands.CreateCustomer;
using Skeleton.Application.Feature.Customer.Commands.DeleteCustomer;
using Skeleton.Application.Feature.Customer.Commands.UpdateCustomer;
using Skeleton.Application.Feature.Customer.CustomerDto;
using Skeleton.Application.Feature.Customer.Queries.GetAll;
using Skeleton.Application.Feature.Customer.Queries.GetById;
using Skeleton.Application.Feature.Customer.Queries.GetByStatus;
using Skeleton.Application.Feature.Customer.Queries.GetPaged;
using Skeleton.Application.Feature.Customer.Queries.Search;

namespace Skeleton.Controllers;

/// <summary>
/// Customers — Manage customer records.
/// </summary>
/// <remarks>
/// **Admin**: full access — create, update, delete, view all.
/// **Employee**: view all customers, search, filter by status.
/// **Customer**: can only view and update their own profile.
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CustomersController : BaseController
{
    private readonly IMediator _mediator;

    public CustomersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Get all customers. **Admin + Employee.**</summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Employee")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllCustomersQuery(), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>Get customers paginated. **Admin + Employee.**</summary>
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
        var result = await _mediator.Send(new GetPagedCustomersQuery(pageNumber, pageSize), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>
    /// Get a customer by ID.
    /// </summary>
    /// <remarks>
    /// **Customer** role: can only access their own profile (matched via JWT CustomerId claim).
    /// </remarks>
    [HttpGet("{customerId:guid}")]
    [Authorize(Roles = "Admin,Employee,Customer")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid customerId, CancellationToken cancellationToken)
    {
        if (User.IsInRole("Customer"))
        {
            var claimId = User.FindFirst("CustomerId")?.Value;
            if (claimId is null || !Guid.TryParse(claimId, out var cId) || cId != customerId)
                return Forbid();
        }

        var result = await _mediator.Send(new GetCustomerByIdQuery(customerId), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>Search customers by name, phone, or email. **Admin + Employee.**</summary>
    [HttpGet("search")]
    [Authorize(Roles = "Admin,Employee")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Search(
        [FromQuery] string keyword,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new SearchCustomersQuery(keyword), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>Filter customers by active/inactive status. **Admin + Employee.**</summary>
    [HttpGet("by-status")]
    [Authorize(Roles = "Admin,Employee")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetByStatus(
        [FromQuery] bool isActive,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetCustomersByStatusQuery(isActive), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>Create a new customer record. **Admin + Employee.**</summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Employee")]
    [EnableRateLimiting("WritePolicy")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create(
        [FromBody] CustomerCreateDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateCustomerCommand(dto), cancellationToken);
        return HandleResult(result, isCreated: true);
    }

    /// <summary>Update a customer record. **Admin only.**</summary>
    [HttpPut]
    [Authorize(Roles = "Admin")]
    [EnableRateLimiting("WritePolicy")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        [FromBody] CustomerUpdateDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateCustomerCommand(dto), cancellationToken);
        return HandleResult(result);
    }

    /// <summary>Delete a customer. **Admin only.**</summary>
    [HttpDelete("{customerId:guid}")]
    [Authorize(Roles = "Admin")]
    [EnableRateLimiting("WritePolicy")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid customerId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteCustomerCommand(customerId), cancellationToken);
        return HandleResult(result);
    }
}
