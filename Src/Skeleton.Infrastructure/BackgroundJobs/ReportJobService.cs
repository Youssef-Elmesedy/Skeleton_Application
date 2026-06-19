using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Skeleton.Application.Services.Interfaces;
using Skeleton.Domain.Eunm;
using Skeleton.Infrastructure.Persistence;

namespace Skeleton.Infrastructure.BackgroundJobs;

public class ReportJobService
{
    private readonly AppDbContext       _ctx;
    private readonly IEmailService      _email;
    private readonly ILogger<ReportJobService> _logger;

    public ReportJobService(AppDbContext ctx, IEmailService email, ILogger<ReportJobService> logger)
    {
        _ctx   = ctx;
        _email = email;
        _logger = logger;
    }

    public async Task SendWeeklySalesReportAsync(CancellationToken ct)
    {
        var weekAgo = DateTime.UtcNow.AddDays(-7);

        var ordersCount   = await _ctx.Orders.CountAsync(o => o.CreateDate >= weekAgo, ct);
        var totalRevenue  = await _ctx.Payments
            .Where(p => p.Status == PaymentStatus.Completed && p.CreateDate >= weekAgo)
            .SumAsync(p => p.Amount, ct);
        var newCustomers  = await _ctx.Customers.CountAsync(c => c.CreateDate >= weekAgo, ct);

        var html = $"""
            <h2>📊 Weekly Sales Report</h2>
            <table style="border-collapse:collapse;width:100%">
              <tr><td>Orders</td><td><strong>{ordersCount}</strong></td></tr>
              <tr><td>Revenue</td><td><strong>{totalRevenue:C}</strong></td></tr>
              <tr><td>New Customers</td><td><strong>{newCustomers}</strong></td></tr>
            </table>
            <p style="color:#6B7280;font-size:12px">Generated: {DateTime.UtcNow:R}</p>
            """;

        var admins = await _ctx.AppUsers
            .Where(u => u.Role == UserRole.Admin)
            .Select(u => u.Email)
            .ToListAsync(ct);

        foreach (var email in admins)
            await _email.SendEmailAsync(email, "Weekly Sales Report 📊", html, ct);

        _logger.LogInformation("[Job] Weekly report sent to {Count} admins", admins.Count);
    }
}
