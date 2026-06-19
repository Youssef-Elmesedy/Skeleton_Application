using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Skeleton.Application.Services.Interfaces;
using Skeleton.Domain.Eunm;
using Skeleton.Infrastructure.Persistence;

namespace Skeleton.Infrastructure.BackgroundJobs;

public class PaymentJobService
{
    private readonly AppDbContext        _ctx;
    private readonly IPaymentService     _paymentService;
    private readonly ILogger<PaymentJobService> _logger;

    public PaymentJobService(
        AppDbContext ctx, IPaymentService paymentService,
        ILogger<PaymentJobService> logger)
    {
        _ctx            = ctx;
        _paymentService = paymentService;
        _logger         = logger;
    }

    /// <summary>Mark payments pending > 30 min as timed-out / expired.</summary>
    public async Task ExpireStalePaymentsAsync(CancellationToken ct)
    {
        var cutoff = DateTime.UtcNow.AddMinutes(-30);
        var stale  = await _ctx.Payments
            .Where(p => p.Status == PaymentStatus.Pending
                     && p.InitiatedAt < cutoff
                     && p.GatewayStatus != PaymentGatewayStatus.Captured)
            .ToListAsync(ct);

        foreach (var payment in stale)
        {
            payment.MarkFailed("Payment session expired.");
            _logger.LogWarning("[Job] Payment {Id} expired (stale > 30 min)", payment.Id);
        }

        if (stale.Any())
            await _ctx.SaveChangesAsync(ct);

        _logger.LogInformation("[Job] Expired {Count} stale payments", stale.Count);
    }

    /// <summary>Retry failed payments that haven't exceeded max retries.</summary>
    public async Task RetryFailedPaymentsAsync(CancellationToken ct)
    {
        var retryable = await _ctx.Payments
            .Where(p => p.Status == PaymentStatus.Failed && p.RetryCount < 3)
            .Select(p => p.Id)
            .ToListAsync(ct);

        foreach (var id in retryable)
        {
            try
            {
                await _paymentService.RetryFailedPaymentAsync(id, ct);
                _logger.LogInformation("[Job] Retried payment {Id}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Job] Retry failed for payment {Id}", id);
            }
        }
    }
}
