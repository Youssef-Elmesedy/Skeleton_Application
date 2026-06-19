using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Skeleton.Application.Services.Interfaces;
using Skeleton.Infrastructure.Persistence;

namespace Skeleton.Infrastructure.BackgroundJobs;

public class InventoryJobService
{
    private readonly AppDbContext _ctx;
    private readonly INotificationService _notifications;
    private readonly ILogger<InventoryJobService> _logger;

    private const int LowStockThreshold = 10;

    public InventoryJobService(
        AppDbContext ctx,
        INotificationService notifications,
        ILogger<InventoryJobService> logger)
    {
        _ctx = ctx;
        _notifications = notifications;
        _logger = logger;
    }

    public async Task CheckLowStockAsync(CancellationToken ct)
    {
        var lowStock = await _ctx.Products
            .Where(p => p.StockQuantity <= LowStockThreshold && p.StockQuantity > 0)
            .Select(p => new { p.Id, p.FullName, p.StockQuantity })
            .ToListAsync(ct);

        foreach (var product in lowStock)
        {
            await _notifications.NotifyLowStockAsync(
                product.Id, product.FullName, product.StockQuantity, ct);

            _logger.LogWarning("[Job] Low stock: {Product} ({Qty} remaining)",
                product.FullName, product.StockQuantity);
        }

        _logger.LogInformation("[Job] Low stock check complete — {Count} alerts", lowStock.Count);
    }
}
