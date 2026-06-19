using Skeleton.Application.Feature.Payment.PaymentDto;

namespace Skeleton.Application.Services.Interfaces;

public interface IPaymentService
{
    // ── Legacy (backward compat) ──────────────────────────────────
    Task<PaymentResponseDto> ProcessPaymentAsync(ProcessPaymentDto dto, CancellationToken ct);
    Task<PaymentResponseDto> RefundPaymentAsync(Guid paymentId, CancellationToken ct);
    Task<IReadOnlyList<PaymentResponseDto>> GetPaymentsByOrderIdAsync(Guid orderId, CancellationToken ct);

    // ── Custom flow ───────────────────────────────────────────────
    Task<PaymentResponseDto>  InitiateAsync(InitiatePaymentDto dto, CancellationToken ct);
    Task<PaymentResponseDto>  ConfirmAsync(ConfirmPaymentDto dto, CancellationToken ct);
    Task<PaymentResponseDto>  RefundWithDetailsAsync(RefundPaymentDto dto, CancellationToken ct);
    Task<bool>                HandleWebhookAsync(PaymentWebhookDto dto, CancellationToken ct);

    // ── Retry (called by Hangfire) ────────────────────────────────
    Task RetryFailedPaymentAsync(Guid paymentId, CancellationToken ct);
}
