using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Skeleton.Application.Services.Interfaces;
using Skeleton.Domain.Eunm;

namespace Skeleton.Infrastructure.Payment.Gateways;

/// <summary>
/// Stripe gateway adapter.
/// Replace stub methods with actual Stripe.net SDK calls:
/// Install: dotnet add package Stripe.net
/// </summary>
public class StripeGatewayService : IPaymentGatewayService
{
    private readonly IConfiguration _config;
    private readonly ILogger<StripeGatewayService> _logger;

    public PaymentGateway Gateway => PaymentGateway.Stripe;

    public StripeGatewayService(IConfiguration config, ILogger<StripeGatewayService> logger)
    {
        _config = config;
        _logger = logger;
        // StripeConfiguration.ApiKey = config["Stripe:SecretKey"];
    }

    public async Task<GatewayInitiateResult> InitiateAsync(
        Guid orderId, decimal amount, string currency,
        string? customerEmail, CancellationToken ct)
    {
        try
        {
            /* --- Real Stripe code (uncomment when Stripe.net is installed) ---
            var options = new PaymentIntentCreateOptions
            {
                Amount             = (long)(amount * 100),  // cents
                Currency           = currency.ToLower(),
                ReceiptEmail       = customerEmail,
                Metadata           = new Dictionary<string, string> { ["orderId"] = orderId.ToString() }
            };
            var service = new PaymentIntentService();
            var intent  = await service.CreateAsync(options, cancellationToken: ct);
            return new GatewayInitiateResult(true, intent.Id, intent.ClientSecret, null);
            ------------------------------------------------------------------- */

            // STUB
            await Task.Delay(50, ct);
            var clientSecret = $"pi_{Guid.NewGuid():N}_secret_{Guid.NewGuid():N}";
            _logger.LogInformation("[Stripe STUB] PaymentIntent created for Order {OrderId}", orderId);
            return new GatewayInitiateResult(true, clientSecret, null, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Stripe] InitiatePayment failed for Order {OrderId}", orderId);
            return new GatewayInitiateResult(false, null, null, ex.Message);
        }
    }

    public async Task<GatewayConfirmResult> ConfirmAsync(
        string gatewayTransactionId, decimal amount, CancellationToken ct)
    {
        try
        {
            /* --- Real Stripe code ---
            var service = new PaymentIntentService();
            var intent  = await service.ConfirmAsync(gatewayTransactionId, cancellationToken: ct);
            var succeeded = intent.Status == "succeeded";
            var fee = CalculateFee(amount);  // 2.9% + $0.30
            return new GatewayConfirmResult(succeeded, intent.Id, fee, intent.ToJson(), null);
            ------------------------------------------------------------------- */

            await Task.Delay(80, ct);
            var fee = Math.Round(amount * 0.029m + 0.30m, 2);
            _logger.LogInformation("[Stripe STUB] Confirmed {TxId}", gatewayTransactionId);
            return new GatewayConfirmResult(true, $"ch_{gatewayTransactionId[..8]}", fee, null, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Stripe] Confirm failed for {TxId}", gatewayTransactionId);
            return new GatewayConfirmResult(false, null, 0, null, ex.Message);
        }
    }

    public async Task<GatewayRefundResult> RefundAsync(
        string gatewayTransactionId, decimal amount, string? reason, CancellationToken ct)
    {
        try
        {
            /* --- Real Stripe code ---
            var options = new RefundCreateOptions
            {
                Charge = gatewayTransactionId,
                Amount = (long)(amount * 100),
                Reason = reason
            };
            var service = new RefundService();
            var refund  = await service.CreateAsync(options, cancellationToken: ct);
            return new GatewayRefundResult(true, refund.Id, null);
            ------------------------------------------------------------------- */

            await Task.Delay(60, ct);
            _logger.LogInformation("[Stripe STUB] Refund {Amount} for {TxId}", amount, gatewayTransactionId);
            return new GatewayRefundResult(true, $"re_{Guid.NewGuid():N}"[..20], null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Stripe] Refund failed");
            return new GatewayRefundResult(false, null, ex.Message);
        }
    }

    public async Task<GatewayWebhookResult> HandleWebhookAsync(
        string rawPayload, string? signature, CancellationToken ct)
    {
        try
        {
            var secret = _config["Stripe:WebhookSecret"];

            // Verify signature (production)
            if (!string.IsNullOrEmpty(secret) && !string.IsNullOrEmpty(signature))
            {
                /* --- Stripe webhook verification ---
                var stripeEvent = EventUtility.ConstructEvent(rawPayload, signature, secret);
                ---------------------------------------- */
            }

            // Parse the event
            using var doc = JsonDocument.Parse(rawPayload);
            var root      = doc.RootElement;
            var eventType = root.GetProperty("type").GetString() ?? "";
            var txId      = root.TryGetProperty("data", out var data)
                         && data.TryGetProperty("object", out var obj)
                         && obj.TryGetProperty("id", out var id)
                            ? id.GetString() : null;

            var (status, failure) = eventType switch
            {
                "payment_intent.succeeded"               => (PaymentGatewayStatus.Captured, (string?)null),
                "payment_intent.payment_failed"          => (PaymentGatewayStatus.Declined, "Payment declined by issuer"),
                "payment_intent.canceled"                => (PaymentGatewayStatus.Cancelled, "Payment cancelled"),
                "charge.refunded"                        => (PaymentGatewayStatus.Refunded, (string?)null),
                _                                        => (PaymentGatewayStatus.Pending, (string?)null)
            };

            return new GatewayWebhookResult(true, eventType, txId, status, failure, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Stripe] Webhook parsing failed");
            return new GatewayWebhookResult(false, null, null, PaymentGatewayStatus.Pending, null, ex.Message);
        }
    }
}
