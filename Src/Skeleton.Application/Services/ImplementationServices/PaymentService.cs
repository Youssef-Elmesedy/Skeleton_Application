using Hangfire;
using Skeleton.Application.Feature.Payment.PaymentDto;
using Skeleton.Domain.Eunm;

namespace Skeleton.Application.Services.ImplementationServices;

public class PaymentService : IPaymentService
{
    private readonly IReadRepository<Payment> _payRead;
    private readonly IWriteRepository<Payment> _payWrite;
    private readonly IReadRepository<Order> _orderRead;
    private readonly IWriteRepository<Order> _orderWrite;
    private readonly IPaymentQueryRepository _queryRepo;
    private readonly IPaymentGatewayFactory _gatewayFactory;
    private readonly INotificationService _notifications;
    private readonly IMapper _mapper;

    public PaymentService(
        IReadRepository<Payment> payRead,
        IWriteRepository<Payment> payWrite,
        IReadRepository<Order> orderRead,
        IWriteRepository<Order> orderWrite,
        IPaymentQueryRepository queryRepo,
        IPaymentGatewayFactory gatewayFactory,
        INotificationService notifications,
        IMapper mapper)
    {
        _payRead = payRead;
        _payWrite = payWrite;
        _orderRead = orderRead;
        _orderWrite = orderWrite;
        _queryRepo = queryRepo;
        _gatewayFactory = gatewayFactory;
        _notifications = notifications;
        _mapper = mapper;
    }

    // ─────────────────────────────────────────────────────────────
    //  STEP 1 — Initiate (create Payment record + call gateway)
    // ─────────────────────────────────────────────────────────────
    public async Task<PaymentResponseDto> InitiateAsync(InitiatePaymentDto dto, CancellationToken ct)
    {
        var order = await _orderRead.GetByIdAsync(dto.OrderId, ct)
                    ?? throw new BusinessException(ErrorType.NotFound, "Order not found.");

        if (order.Status != OrderStatus.Pending)
            throw new BusinessException(ErrorType.Validation, "Only pending orders can be paid.");

        var payment = new Payment(order.Id, order.TotalAmount, dto.Method, dto.Gateway);

        if (dto.InstallmentMonths.HasValue)
            payment.SetInstallment(dto.InstallmentMonths.Value);

        // Call gateway
        var gateway = _gatewayFactory.GetGateway(dto.Gateway);
        var init = await gateway.InitiateAsync(
            order.Id, order.TotalAmount, "EGP",
            order.Customer?.Email, ct);

        if (!init.Success)
        {
            payment.MarkFailed(init.Error ?? "Gateway initiation failed.");
        }
        else
        {
            payment.SetGatewayPending(init.GatewayReferenceCode);
        }

        await _payWrite.AddAsync(payment);
        await _payWrite.SaveChangesAsync(ct);

        // Schedule auto-expire after 30 minutes
        BackgroundJob.Schedule<IPaymentService>(
            svc => svc.RetryFailedPaymentAsync(payment.Id, CancellationToken.None),
            TimeSpan.FromMinutes(30));

        return MapToDto(payment);
    }

    // ─────────────────────────────────────────────────────────────
    //  STEP 2 — Confirm (after user completes gateway UI)
    // ─────────────────────────────────────────────────────────────
    public async Task<PaymentResponseDto> ConfirmAsync(ConfirmPaymentDto dto, CancellationToken ct)
    {
        var payment = await _payRead.GetByIdAsync(dto.PaymentId, ct)
                      ?? throw new BusinessException(ErrorType.NotFound, "Payment not found.");

        if (payment.Status != PaymentStatus.Pending)
            throw new BusinessException(ErrorType.Validation, "Payment is not in pending state.");

        var gateway = _gatewayFactory.GetGateway(payment.Gateway);
        var result = await gateway.ConfirmAsync(dto.GatewayTransactionId, payment.Amount, ct);

        if (result.Success)
        {
            payment.MarkSuccess(result.TransactionId!, result.Fee, result.RawResponse);

            var order = await _orderRead.GetByIdAsync(payment.OrderId, ct);
            if (order is not null)
            {
                order.UpdateStatus(OrderStatus.Confirmed);
                await _orderWrite.UpdateAsync(order);

                // Notify customer
                BackgroundJob.Enqueue<INotificationService>(svc =>
                    svc.NotifyPaymentAsync(order.CustomerId, payment.Id, PaymentStatus.Completed, CancellationToken.None));
                BackgroundJob.Enqueue<INotificationService>(svc =>
                    svc.NotifyOrderStatusAsync(order.CustomerId, order.Id, OrderStatus.Confirmed, CancellationToken.None));
            }
        }
        else
        {
            payment.MarkFailed(result.Error ?? "Gateway confirmation failed.");
            var order = await _orderRead.GetByIdAsync(payment.OrderId, ct);
            if (order is not null)
            {
                BackgroundJob.Enqueue<INotificationService>(svc =>
                    svc.NotifyPaymentAsync(order.CustomerId, payment.Id, PaymentStatus.Failed, CancellationToken.None));
            }
        }

        await _payWrite.UpdateAsync(payment);
        await _payWrite.SaveChangesAsync(ct);

        return MapToDto(payment);
    }

    // ─────────────────────────────────────────────────────────────
    //  STEP 3 — Webhook (gateway → our server)
    // ─────────────────────────────────────────────────────────────
    public async Task<bool> HandleWebhookAsync(PaymentWebhookDto dto, CancellationToken ct)
    {
        if (!Enum.TryParse<PaymentGateway>(dto.Gateway, ignoreCase: true, out var gatewayEnum))
            return false;

        var gateway = _gatewayFactory.GetGateway(gatewayEnum);
        var result = await gateway.HandleWebhookAsync(dto.RawPayload, dto.Signature, ct);

        if (!result.Success || result.GatewayTransactionId is null) return false;

        // Find payment by gateway transaction ID
        var payment = await _payRead.FirstOrDefaultAsync(
            p => p.GatewayTransactionId == result.GatewayTransactionId, ct);

        if (payment is null) return false;

        payment.RecordWebhook(dto.RawPayload);

        switch (result.Status)
        {
            case PaymentGatewayStatus.Captured:
                payment.MarkSuccess(result.GatewayTransactionId);
                var order = await _orderRead.GetByIdAsync(payment.OrderId, ct);
                if (order is not null)
                {
                    order.UpdateStatus(OrderStatus.Confirmed);
                    await _orderWrite.UpdateAsync(order);
                    BackgroundJob.Enqueue<INotificationService>(svc =>
                        svc.NotifyPaymentAsync(order.CustomerId, payment.Id, PaymentStatus.Completed, CancellationToken.None));
                }
                break;
            case PaymentGatewayStatus.Declined:
            case PaymentGatewayStatus.TimedOut:
                payment.MarkFailed(result.FailureReason ?? "Declined by gateway.");
                break;
            case PaymentGatewayStatus.Refunded:
                payment.MarkRefunded();
                break;
        }

        await _payWrite.UpdateAsync(payment);
        await _payWrite.SaveChangesAsync(ct);
        return true;
    }

    // ─────────────────────────────────────────────────────────────
    //  Refund with details
    // ─────────────────────────────────────────────────────────────
    public async Task<PaymentResponseDto> RefundWithDetailsAsync(RefundPaymentDto dto, CancellationToken ct)
    {
        var payment = await _payRead.GetByIdAsync(dto.PaymentId, ct)
                      ?? throw new BusinessException(ErrorType.NotFound, "Payment not found.");

        var gateway = _gatewayFactory.GetGateway(payment.Gateway);
        var amount = dto.Amount ?? payment.Amount;

        var result = await gateway.RefundAsync(
            payment.GatewayTransactionId ?? payment.Id.ToString(), amount, dto.Reason, ct);

        if (!result.Success)
            throw new BusinessException(ErrorType.Validation, result.Error ?? "Refund failed.");

        payment.MarkRefunded(amount, dto.Reason);

        var order = await _orderRead.GetByIdAsync(payment.OrderId, ct);
        if (order is not null)
        {
            order.UpdateStatus(OrderStatus.Refunded);
            await _orderWrite.UpdateAsync(order);
            BackgroundJob.Enqueue<INotificationService>(svc =>
                svc.NotifyPaymentAsync(order.CustomerId, payment.Id, PaymentStatus.Refunded, CancellationToken.None));
        }

        await _payWrite.UpdateAsync(payment);
        await _payWrite.SaveChangesAsync(ct);

        return MapToDto(payment);
    }

    // ─────────────────────────────────────────────────────────────
    //  Retry (called by Hangfire background job)
    // ─────────────────────────────────────────────────────────────
    public async Task RetryFailedPaymentAsync(Guid paymentId, CancellationToken ct)
    {
        var payment = await _payRead.GetByIdAsync(paymentId, ct);
        if (payment is null || payment.Status != PaymentStatus.Pending) return;

        payment.IncrementRetry();

        var gateway = _gatewayFactory.GetGateway(payment.Gateway);
        var result = await gateway.ConfirmAsync(
            payment.GatewayTransactionId ?? payment.Id.ToString(), payment.Amount, ct);

        if (result.Success)
            payment.MarkSuccess(result.TransactionId!, result.Fee, result.RawResponse);
        else if (payment.RetryCount >= 3)
            payment.MarkFailed("Max retries reached. Payment expired.");

        await _payWrite.UpdateAsync(payment);
        await _payWrite.SaveChangesAsync(ct);
    }

    // ─────────────────────────────────────────────────────────────
    //  Legacy methods
    // ─────────────────────────────────────────────────────────────
    public async Task<PaymentResponseDto> ProcessPaymentAsync(ProcessPaymentDto dto, CancellationToken ct)
    {
        var init = await InitiateAsync(
            new InitiatePaymentDto(dto.OrderId, dto.Method, dto.Gateway), ct);

        return await ConfirmAsync(
            new ConfirmPaymentDto(init.Id, $"LEGACY-{Guid.NewGuid():N}"[..20]), ct);
    }

    public async Task<PaymentResponseDto> RefundPaymentAsync(Guid paymentId, CancellationToken ct)
        => await RefundWithDetailsAsync(new RefundPaymentDto(paymentId), ct);

    public Task<IReadOnlyList<PaymentResponseDto>> GetPaymentsByOrderIdAsync(Guid orderId, CancellationToken ct)
        => _queryRepo.GetPaymentsByOrderIdAsync(orderId, ct);

    // ─────────────────────────────────────────────────────────────
    //  Mapping
    // ─────────────────────────────────────────────────────────────
    private static PaymentResponseDto MapToDto(Payment p) => new(
        p.Id, p.OrderId, p.Amount, p.GatewayFee, p.NetAmount,
        p.Method, p.Gateway, p.Status, p.GatewayStatus,
        p.GatewayTransactionId, p.GatewayReferenceCode,
        p.FailureReason, p.RefundReason, p.RefundAmount,
        p.IsInstallment, p.InstallmentMonths, p.MonthlyAmount,
        p.InitiatedAt, p.CompletedAt, p.RefundedAt, p.PaidAt, p.CreateDate);
}
