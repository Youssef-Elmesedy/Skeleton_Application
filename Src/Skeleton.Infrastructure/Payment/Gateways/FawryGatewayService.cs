using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Skeleton.Application.Services.Interfaces;
using Skeleton.Domain.Eunm;
using System.Security.Cryptography;
using System.Text;

namespace Skeleton.Infrastructure.Payment.Gateways;

/// <summary>
/// Fawry payment gateway — Egyptian market.
/// Generates a reference code that the customer pays at any Fawry kiosk.
/// Production docs: https://developer.fawrystaging.com
/// </summary>
public class FawryGatewayService : IPaymentGatewayService
{
    private readonly IConfiguration _config;
    private readonly ILogger<FawryGatewayService> _logger;
    private readonly HttpClient _http;

    public PaymentGateway Gateway => PaymentGateway.Fawry;

    public FawryGatewayService(
        IConfiguration config,
        ILogger<FawryGatewayService> logger,
        IHttpClientFactory httpFactory)
    {
        _config = config;
        _logger = logger;
        _http   = httpFactory.CreateClient("Fawry");
    }

    public async Task<GatewayInitiateResult> InitiateAsync(
        Guid orderId, decimal amount, string currency,
        string? customerEmail, CancellationToken ct)
    {
        try
        {
            var merchantCode   = _config["Fawry:MerchantCode"];
            var securityKey    = _config["Fawry:SecurityKey"];
            var referenceCode  = $"ORD-{orderId.ToString()[..8].ToUpper()}";

            // Signature: SHA256(merchantCode + refCode + amount + securityKey)
            var signature = ComputeSignature(merchantCode!, referenceCode, amount, securityKey!);

            /* Production API call:
            var payload = new { merchantCode, referenceCode, amount, signature ... };
            var resp = await _http.PostAsJsonAsync("/fawrypay-apis/api/payments/init", payload, ct);
            var result = await resp.Content.ReadFromJsonAsync<FawryResponse>(ct);
            return new GatewayInitiateResult(result.StatusCode == 200, result.ReferenceNumber, null, result.StatusDescription);
            */

            await Task.Delay(40, ct);
            _logger.LogInformation("[Fawry STUB] Reference {Ref} for Order {OrderId}", referenceCode, orderId);
            return new GatewayInitiateResult(true, referenceCode, null, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Fawry] Initiate failed for Order {OrderId}", orderId);
            return new GatewayInitiateResult(false, null, null, ex.Message);
        }
    }

    public Task<GatewayConfirmResult> ConfirmAsync(
        string gatewayTransactionId, decimal amount, CancellationToken ct)
        // Fawry notifies via webhook; no active confirm needed
        => Task.FromResult(new GatewayConfirmResult(true, gatewayTransactionId, 0, null, null));

    public async Task<GatewayRefundResult> RefundAsync(
        string gatewayTransactionId, decimal amount, string? reason, CancellationToken ct)
    {
        await Task.Delay(40, ct);
        _logger.LogInformation("[Fawry STUB] Refund {Amount} for {Ref}", amount, gatewayTransactionId);
        return new GatewayRefundResult(true, $"FREF-{Guid.NewGuid().ToString()[..8].ToUpper()}", null);
    }

    public async Task<GatewayWebhookResult> HandleWebhookAsync(
        string rawPayload, string? signature, CancellationToken ct)
    {
        try
        {
            using var doc  = System.Text.Json.JsonDocument.Parse(rawPayload);
            var root       = doc.RootElement;
            var statusCode = root.TryGetProperty("paymentStatus", out var ps) ? ps.GetString() : null;
            var refCode    = root.TryGetProperty("referenceNumber", out var rn) ? rn.GetString() : null;

            var status = statusCode switch
            {
                "PAID"            => PaymentGatewayStatus.Captured,
                "UNPAID"          => PaymentGatewayStatus.Pending,
                "EXPIRED"         => PaymentGatewayStatus.TimedOut,
                "REFUNDED"        => PaymentGatewayStatus.Refunded,
                _                 => PaymentGatewayStatus.Pending
            };

            return new GatewayWebhookResult(true, statusCode, refCode, status, null, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Fawry] Webhook parsing failed");
            return new GatewayWebhookResult(false, null, null, PaymentGatewayStatus.Pending, null, ex.Message);
        }
    }

    private static string ComputeSignature(string merchantCode, string refCode, decimal amount, string securityKey)
    {
        var raw   = $"{merchantCode}{refCode}{amount:F2}{securityKey}";
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(raw));
        return Convert.ToHexString(bytes).ToLower();
    }
}
