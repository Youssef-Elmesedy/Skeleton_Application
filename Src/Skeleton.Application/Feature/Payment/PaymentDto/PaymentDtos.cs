using Skeleton.Domain.Eunm;

namespace Skeleton.Application.Feature.Payment.PaymentDto;

// ── Read ──────────────────────────────────────────────────────────
public record PaymentResponseDto(
    Guid                Id,
    Guid                OrderId,
    decimal             Amount,
    decimal             GatewayFee,
    decimal             NetAmount,
    PaymentMethod       Method,
    PaymentGateway      Gateway,
    PaymentStatus       Status,
    PaymentGatewayStatus GatewayStatus,
    string?             GatewayTransactionId,
    string?             GatewayReferenceCode,
    string?             FailureReason,
    string?             RefundReason,
    decimal             RefundAmount,
    bool                IsInstallment,
    int                 InstallmentMonths,
    decimal             MonthlyAmount,
    DateTime            InitiatedAt,
    DateTime?           CompletedAt,
    DateTime?           RefundedAt,
    DateTime?           PaidAt,
    DateTime?           CreateDate
);

// ── Initiate (step 1) ─────────────────────────────────────────────
public record InitiatePaymentDto(
    Guid           OrderId,
    PaymentMethod  Method,
    PaymentGateway Gateway,
    int?           InstallmentMonths = null  // optional — for Gateway = Installment
);

// ── Confirm / Capture (step 2 — after user completes gateway UI) ──
public record ConfirmPaymentDto(
    Guid    PaymentId,
    string  GatewayTransactionId,   // token/charge ID returned by gateway
    string? RawGatewayResponse = null
);

// ── Webhook (step 3 — gateway callback) ──────────────────────────
public record PaymentWebhookDto(
    string  Gateway,        // "stripe" | "fawry" | etc.
    string  RawPayload,     // raw JSON body
    string? Signature       // e.g. Stripe-Signature header
);

// ── Refund ────────────────────────────────────────────────────────
public record RefundPaymentDto(
    Guid    PaymentId,
    decimal? Amount  = null,   // null = full refund
    string?  Reason  = null
);

// ── Legacy (kept for compatibility) ──────────────────────────────
public record ProcessPaymentDto(
    Guid           OrderId,
    PaymentMethod  Method,
    PaymentGateway Gateway = PaymentGateway.Cash
);

// ── Summary for order response ────────────────────────────────────
public record PaymentSummaryDto(
    Guid         Id,
    decimal      Amount,
    PaymentStatus Status,
    string?      TransactionId,
    DateTime?    PaidAt
);
