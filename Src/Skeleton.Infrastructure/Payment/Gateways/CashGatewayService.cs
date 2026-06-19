using Skeleton.Application.Services.Interfaces;
using Skeleton.Domain.Eunm;

namespace Skeleton.Infrastructure.Payment.Gateways;

/// <summary>Cash / in-store payment — auto-succeeds immediately.</summary>
public class CashGatewayService : IPaymentGatewayService
{
    public PaymentGateway Gateway => PaymentGateway.Cash;

    public Task<GatewayInitiateResult> InitiateAsync(
        Guid orderId, decimal amount, string currency,
        string? customerEmail, CancellationToken ct)
        => Task.FromResult(new GatewayInitiateResult(
            true, $"CASH-{orderId.ToString()[..8].ToUpper()}", null, null));

    public Task<GatewayConfirmResult> ConfirmAsync(
        string gatewayTransactionId, decimal amount, CancellationToken ct)
        => Task.FromResult(new GatewayConfirmResult(true, gatewayTransactionId, 0, null, null));

    public Task<GatewayRefundResult> RefundAsync(
        string gatewayTransactionId, decimal amount, string? reason, CancellationToken ct)
        => Task.FromResult(new GatewayRefundResult(true, $"REF-{Guid.NewGuid().ToString()[..8].ToUpper()}", null));

    public Task<GatewayWebhookResult> HandleWebhookAsync(
        string rawPayload, string? signature, CancellationToken ct)
        => Task.FromResult(new GatewayWebhookResult(false, null, null, PaymentGatewayStatus.Pending, null, "Cash does not use webhooks"));
}
