using Skeleton.Application.Feature.Payment.PaymentDto;
using Skeleton.Domain.Eunm;

namespace Skeleton.Application.Services.Interfaces;

public interface IPaymentGatewayService
{
    PaymentGateway Gateway { get; }

    /// <summary>Initiate a payment — returns redirect URL or ref code</summary>
    Task<GatewayInitiateResult> InitiateAsync(
        Guid orderId, decimal amount, string currency,
        string? customerEmail, CancellationToken ct);

    /// <summary>Capture / confirm the payment after user action</summary>
    Task<GatewayConfirmResult> ConfirmAsync(
        string gatewayTransactionId, decimal amount, CancellationToken ct);

    /// <summary>Process refund</summary>
    Task<GatewayRefundResult> RefundAsync(
        string gatewayTransactionId, decimal amount, string? reason, CancellationToken ct);

    /// <summary>Verify and parse a webhook payload</summary>
    Task<GatewayWebhookResult> HandleWebhookAsync(
        string rawPayload, string? signature, CancellationToken ct);
}

// ── Result records ────────────────────────────────────────────────
public record GatewayInitiateResult(
    bool    Success,
    string? GatewayReferenceCode,
    string? RedirectUrl,
    string? Error
);

public record GatewayConfirmResult(
    bool    Success,
    string? TransactionId,
    decimal Fee,
    string? RawResponse,
    string? Error
);

public record GatewayRefundResult(
    bool    Success,
    string? RefundId,
    string? Error
);

public record GatewayWebhookResult(
    bool                 Success,
    string?              EventType,          // "payment.captured", "payment.failed", etc.
    string?              GatewayTransactionId,
    PaymentGatewayStatus Status,
    string?              FailureReason,
    string?              Error
);
