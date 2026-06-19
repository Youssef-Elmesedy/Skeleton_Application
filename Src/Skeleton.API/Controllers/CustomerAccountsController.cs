using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Skeleton.Application.Feature.CustomerAccount.AccountDto;
using Skeleton.Application.Feature.CustomerAccount.Commands.AddDebit;
using Skeleton.Application.Feature.CustomerAccount.Commands.AddPayment;
using Skeleton.Application.Feature.CustomerAccount.Queries.GetAccount;

namespace Skeleton.Controllers;

/// <summary>
/// Customer Accounts — Manage customer balances and transaction history.
/// </summary>
/// <remarks>
/// Tracks **credit/debit transactions** for each customer.
///
/// | Role         | Access |
/// |--------------|--------|
/// | **Admin**    | Full access — view and modify any account |
/// | **Employee** | View and modify any account |
/// | **Customer** | View their own account only (read-only) |
///
/// > **Note**: This is separate from order payments.
/// > A debit means the customer owes money (purchased on credit).
/// > A payment reduces the customer's outstanding balance.
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CustomerAccountsController : BaseController
{
    private readonly IMediator _mediator;

    public CustomerAccountsController(IMediator mediator)
        => _mediator = mediator;

    /// <summary>
    /// Get the account balance and full transaction history for a customer.
    /// </summary>
    /// <remarks>
    /// - **Admin / Employee**: can query any customer's account.
    /// - **Customer**: can only access their own account (matched via JWT `CustomerId` claim).
    /// </remarks>
    [HttpGet("{customerId:guid}")]
    [Authorize(Roles = "Admin,Employee,Customer")]
    [ProducesResponseType(typeof(ApiResponse<CustomerAccountResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAccount(
        Guid customerId,
        CancellationToken cancellationToken)
    {
        if (User.IsInRole("Customer") && !IsOwnCustomer(customerId))
            return Forbid();

        var result = await _mediator.Send(
            new GetAccountByCustomerIdQuery(customerId), cancellationToken);

        return HandleResult(result);
    }

    /// <summary>
    /// Add a debit (debt) to a customer's account. **Admin + Employee.**
    /// </summary>
    /// <remarks>
    /// Use when the customer purchases goods **on credit**.
    /// The balance increases (customer owes more).
    ///
    /// Example: customer takes 500 EGP worth of goods without paying → `Amount: 500`
    /// </remarks>
    [HttpPost("{customerId:guid}/debit")]
    [Authorize(Roles = "Admin,Employee")]
    [EnableRateLimiting("WritePolicy")]
    [ProducesResponseType(typeof(ApiResponse<CustomerAccountResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>),                    StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddDebit(
        Guid customerId,
        [FromBody] AddDebitDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new AddDebitCommand(customerId, dto), cancellationToken);

        return HandleResult(result);
    }

    /// <summary>
    /// Record a payment from a customer. **Admin + Employee.**
    /// </summary>
    /// <remarks>
    /// Use when the customer **pays off** part or all of their outstanding balance.
    /// The balance decreases.
    ///
    /// Example: customer had 500 EGP debt and pays 200 EGP → `Amount: 200`
    ///
    /// > Returns `400 Bad Request` if `Amount` exceeds current balance.
    /// </remarks>
    [HttpPost("{customerId:guid}/payment")]
    [Authorize(Roles = "Admin,Employee")]
    [EnableRateLimiting("WritePolicy")]
    [ProducesResponseType(typeof(ApiResponse<CustomerAccountResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>),                    StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddPayment(
        Guid customerId,
        [FromBody] AddPaymentDto dto,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new AddPaymentCommand(customerId, dto), cancellationToken);

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
