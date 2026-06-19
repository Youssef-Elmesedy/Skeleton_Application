using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Skeleton.Application.Feature.Payment.PaymentDto;
using Skeleton.Application.Services.Interfaces;

namespace Skeleton.Controllers;

/// <summary>
/// Payments — Multi-step custom payment flow.
/// </summary>
/// <remarks>
/// ### Payment Flow
/// ```
/// 1. POST /api/payments/initiate   → creates Payment record, calls gateway
/// 2. POST /api/payments/confirm    → captures payment after user completes gateway UI
/// 3. POST /api/payments/webhook    → gateway callback (async notification)
/// ```
///
/// ### Supported Gateways
/// | Gateway | Description |
/// |---------|-------------|
/// | `Cash` | In-store / instant |
/// | `Stripe` | Credit/Debit card (online) |
/// | `Fawry` | Egyptian kiosk payments |
/// | `BankTransfer` | Direct bank transfer |
/// | `Installment` | Deferred installment plan |
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentsController : BaseController
{
    private readonly IPaymentService _service;

    public PaymentsController(IPaymentService service) => _service = service;

    // ── STEP 1: Initiate ─────────────────────────────────────────

    /// <summary>
    /// Initiate a payment — creates the payment record and calls the gateway.
    /// </summary>
    /// <remarks>
    /// Returns a `GatewayReferenceCode` (e.g. Fawry kiosk code) or
    /// `GatewayTransactionId` (e.g. Stripe PaymentIntent client_secret)
    /// for the client to use in the next step.
    ///
    /// **Admin + Employee only.**
    /// </remarks>
    [HttpPost("initiate")]
    [Authorize(Roles = "Admin,Employee")]
    [EnableRateLimiting("WritePolicy")]
    [ProducesResponseType(typeof(ApiResponse<PaymentResponseDto>), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Initiate(
        [FromBody] InitiatePaymentDto dto, CancellationToken ct)
    {
        var result = await _service.InitiateAsync(dto, ct);
        return StatusCode(201, ApiResponse<PaymentResponseDto>.Success(
            "Payment initiated. Complete the payment on the gateway.", result));
    }

    // ── STEP 2: Confirm ──────────────────────────────────────────

    /// <summary>
    /// Confirm/capture the payment after the user completes the gateway UI.
    /// </summary>
    /// <remarks>
    /// For Stripe: pass the `paymentIntentId` returned by `stripe.confirmPayment()`.
    /// For Fawry: this is called automatically via webhook.
    ///
    /// On success, the order status moves to **Confirmed**.
    /// Customer receives a real-time notification via SignalR.
    ///
    /// **Admin + Employee only.**
    /// </remarks>
    [HttpPost("confirm")]
    [Authorize(Roles = "Admin,Employee")]
    [EnableRateLimiting("WritePolicy")]
    [ProducesResponseType(typeof(ApiResponse<PaymentResponseDto>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Confirm(
        [FromBody] ConfirmPaymentDto dto, CancellationToken ct)
    {
        var result = await _service.ConfirmAsync(dto, ct);
        return Ok(ApiResponse<PaymentResponseDto>.Success("Payment confirmed.", result));
    }

    // ── STEP 3: Webhook ──────────────────────────────────────────

    /// <summary>
    /// Gateway webhook endpoint — receives async payment status from the gateway.
    /// </summary>
    /// <remarks>
    /// This endpoint is called **by the payment gateway** (Stripe, Fawry, etc.),
    /// not by the client. It should be publicly accessible (no [Authorize]).
    ///
    /// Register this URL in your gateway dashboard:
    /// `https://yourdomain.com/api/payments/webhook`
    ///
    /// Pass `gateway=stripe` or `gateway=fawry` in the query string.
    /// </remarks>
    [HttpPost("webhook")]
    [AllowAnonymous]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Webhook(
        [FromQuery] string gateway,
        CancellationToken ct)
    {
        using var reader = new StreamReader(Request.Body);
        var rawPayload   = await reader.ReadToEndAsync(ct);
        var signature    = Request.Headers["Stripe-Signature"].FirstOrDefault()
                        ?? Request.Headers["Fawry-Signature"].FirstOrDefault();

        var dto    = new PaymentWebhookDto(gateway, rawPayload, signature);
        var result = await _service.HandleWebhookAsync(dto, ct);

        return result ? Ok(new { received = true }) : BadRequest(new { received = false });
    }

    // ── Refund ───────────────────────────────────────────────────

    /// <summary>Refund a payment. Admin only. Supports partial refund.</summary>
    /// <remarks>
    /// Pass `amount` for partial refund; omit for full refund.
    /// The gateway is called to process the actual refund.
    /// Customer receives notification automatically.
    /// </remarks>
    [HttpPost("refund")]
    [Authorize(Roles = "Admin")]
    [EnableRateLimiting("WritePolicy")]
    [ProducesResponseType(typeof(ApiResponse<PaymentResponseDto>), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Refund(
        [FromBody] RefundPaymentDto dto, CancellationToken ct)
    {
        var result = await _service.RefundWithDetailsAsync(dto, ct);
        return Ok(ApiResponse<PaymentResponseDto>.Success("Refund processed successfully.", result));
    }

    // ── Query ────────────────────────────────────────────────────

    /// <summary>Get all payments for a specific order.</summary>
    [HttpGet("by-order/{orderId:guid}")]
    [Authorize(Roles = "Admin,Employee,Customer")]
    [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<PaymentResponseDto>>), 200)]
    public async Task<IActionResult> GetByOrder(Guid orderId, CancellationToken ct)
    {
        var result = await _service.GetPaymentsByOrderIdAsync(orderId, ct);
        return Ok(ApiResponse<IReadOnlyList<PaymentResponseDto>>.Success("Payments retrieved.", result));
    }

    // ── Legacy (compatibility) ────────────────────────────────────

    /// <summary>
    /// [Legacy] Process a cash/simple payment in one step.
    /// Prefer the Initiate → Confirm flow for gateway payments.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Employee")]
    [EnableRateLimiting("WritePolicy")]
    [ProducesResponseType(typeof(ApiResponse<PaymentResponseDto>), 201)]
    public async Task<IActionResult> Process(
        [FromBody] ProcessPaymentDto dto, CancellationToken ct)
    {
        var result = await _service.ProcessPaymentAsync(dto, ct);
        return StatusCode(201, ApiResponse<PaymentResponseDto>.Success("Payment processed.", result));
    }
}
