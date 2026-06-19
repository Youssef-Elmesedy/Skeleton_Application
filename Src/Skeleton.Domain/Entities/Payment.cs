using Skeleton.Domain.Eunm;
using Skeleton.Domain.Exceptions;

namespace Skeleton.Domain.Entities;

/// <summary>
/// Represents a full payment lifecycle including gateway interaction,
/// webhooks, installment tracking and refunds.
/// </summary>
public class Payment : BaseEntity
{
    // ── Core ─────────────────────────────────────────────────────
    public Guid          OrderId    { get; private set; }
    public decimal       Amount     { get; private set; }
    public PaymentMethod Method     { get; private set; }
    public PaymentStatus Status     { get; private set; }

    // ── Gateway ───────────────────────────────────────────────────
    public PaymentGateway       Gateway         { get; private set; }
    public PaymentGatewayStatus GatewayStatus   { get; private set; }
    public string? GatewayTransactionId         { get; private set; }  // External ID from Stripe/Fawry
    public string? GatewayReferenceCode         { get; private set; }  // Human-readable ref
    public string? GatewayRawResponse           { get; private set; }  // JSON response
    public string? WebhookPayload               { get; private set; }  // Raw webhook body

    // ── Fees & net ────────────────────────────────────────────────
    public decimal GatewayFee    { get; private set; }  // e.g. Stripe 2.9% + 30¢
    public decimal NetAmount     => Amount - GatewayFee;

    // ── Installments ──────────────────────────────────────────────
    public bool   IsInstallment      { get; private set; }
    public int    InstallmentMonths  { get; private set; }
    public decimal MonthlyAmount     { get; private set; }

    // ── Refund ────────────────────────────────────────────────────
    public decimal  RefundAmount  { get; private set; }
    public string?  RefundReason  { get; private set; }
    public DateTime? RefundedAt   { get; private set; }

    // ── Failure ───────────────────────────────────────────────────
    public string?  FailureReason    { get; private set; }
    public int      RetryCount       { get; private set; }
    public DateTime? LastRetryAt     { get; private set; }

    // ── Timestamps ───────────────────────────────────────────────
    public DateTime  InitiatedAt   { get; private set; }
    public DateTime? CompletedAt   { get; private set; }
    public DateTime? PaidAt        { get; private set; }  // kept for compatibility

    // ── Navigation ───────────────────────────────────────────────
    public Order Order { get; private set; } = null!;

    // ── Constructor ──────────────────────────────────────────────
    public Payment(Guid orderId, decimal amount, PaymentMethod method, PaymentGateway gateway)
    {
        if (orderId == Guid.Empty)
            throw new BusinessException(ErrorType.Validation, "OrderId is required.");
        if (amount <= 0)
            throw new BusinessException(ErrorType.Validation, "Payment amount must be greater than zero.");

        OrderId       = orderId;
        Amount        = amount;
        Method        = method;
        Gateway       = gateway;
        Status        = PaymentStatus.Pending;
        GatewayStatus = PaymentGatewayStatus.Initiated;
        InitiatedAt   = DateTime.UtcNow;
    }

    // ── Gateway lifecycle ─────────────────────────────────────────
    public void SetGatewayPending(string? referenceCode = null)
    {
        GatewayStatus      = PaymentGatewayStatus.Pending;
        GatewayReferenceCode = referenceCode;
    }

    public void MarkAuthorized(string gatewayTransactionId, string? rawResponse = null)
    {
        GatewayStatus         = PaymentGatewayStatus.Authorized;
        GatewayTransactionId  = gatewayTransactionId;
        GatewayRawResponse    = rawResponse;
    }

    public void MarkSuccess(string transactionReference, decimal fee = 0, string? rawResponse = null)
    {
        Status               = PaymentStatus.Completed;
        GatewayStatus        = PaymentGatewayStatus.Captured;
        GatewayTransactionId = transactionReference;
        GatewayFee           = fee;
        GatewayRawResponse   = rawResponse;
        CompletedAt          = DateTime.UtcNow;
        PaidAt               = DateTime.UtcNow;
    }

    public void MarkFailed(string reason, string? rawResponse = null)
    {
        Status             = PaymentStatus.Failed;
        GatewayStatus      = PaymentGatewayStatus.Declined;
        FailureReason      = reason;
        GatewayRawResponse = rawResponse;
    }

    public void RecordWebhook(string payload)
        => WebhookPayload = payload;

    public void IncrementRetry()
    {
        RetryCount++;
        LastRetryAt = DateTime.UtcNow;
    }

    // ── Refund ────────────────────────────────────────────────────
    public void MarkRefunded(decimal? refundAmount = null, string? reason = null)
    {
        if (Status != PaymentStatus.Completed)
            throw new BusinessException(ErrorType.Validation, "Only completed payments can be refunded.");

        var actual = refundAmount ?? Amount;
        if (actual > Amount)
            throw new BusinessException(ErrorType.Validation, "Refund cannot exceed original amount.");

        Status        = PaymentStatus.Refunded;
        GatewayStatus = PaymentGatewayStatus.Refunded;
        RefundAmount  = actual;
        RefundReason  = reason;
        RefundedAt    = DateTime.UtcNow;
    }

    // ── Installment setup ─────────────────────────────────────────
    public void SetInstallment(int months)
    {
        if (months < 2 || months > 60)
            throw new BusinessException(ErrorType.Validation, "Installment months must be between 2 and 60.");

        IsInstallment     = true;
        InstallmentMonths = months;
        MonthlyAmount     = Math.Round(Amount / months, 2);
    }
}
