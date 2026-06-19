using Hangfire;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Skeleton.Infrastructure.BackgroundJobs;

/// <summary>
/// Registers all recurring Hangfire jobs.
/// Called once at app startup.
/// </summary>
public static class RecurringJobsSetup
{
    public static void RegisterAll(IConfiguration config)
    {
        // ── Every hour: expire pending payments older than 30 min ──
        RecurringJob.AddOrUpdate<PaymentJobService>(
            "expire-stale-payments",
            svc => svc.ExpireStalePaymentsAsync(CancellationToken.None),
            Cron.Hourly());

        // ── Every 5 min: retry failed payments (max 3 attempts) ───
        RecurringJob.AddOrUpdate<PaymentJobService>(
            "retry-failed-payments",
            svc => svc.RetryFailedPaymentsAsync(CancellationToken.None),
            "*/5 * * * *");

        // ── Daily at 2 AM: check low stock ──────────────────────
        RecurringJob.AddOrUpdate<InventoryJobService>(
            "check-low-stock",
            svc => svc.CheckLowStockAsync(CancellationToken.None),
            Cron.Daily(2));

        // ── Daily at 3 AM: cleanup old notifications (>90 days) ──
        RecurringJob.AddOrUpdate<CleanupJobService>(
            "cleanup-old-notifications",
            svc => svc.CleanupOldNotificationsAsync(CancellationToken.None),
            Cron.Daily(3));

        // ── Weekly: generate sales summary report ─────────────────
        RecurringJob.AddOrUpdate<ReportJobService>(
            "weekly-sales-report",
            svc => svc.SendWeeklySalesReportAsync(CancellationToken.None),
            Cron.Weekly(DayOfWeek.Monday, 8));
    }
}
