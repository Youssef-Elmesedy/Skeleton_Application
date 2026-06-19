using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Skeleton.Infrastructure.Persistence;

namespace Skeleton.Infrastructure.BackgroundJobs;

public class CleanupJobService
{
    private readonly AppDbContext _ctx;
    private readonly ILogger<CleanupJobService> _logger;

    public CleanupJobService(AppDbContext ctx, ILogger<CleanupJobService> logger)
    {
        _ctx   = ctx;
        _logger = logger;
    }

    public async Task CleanupOldNotificationsAsync(CancellationToken ct)
    {
        var cutoff = DateTime.UtcNow.AddDays(-90);
        var old    = await _ctx.Notifications
            .Where(n => n.IsRead && n.ReadAt < cutoff)
            .ToListAsync(ct);

        _ctx.Notifications.RemoveRange(old);
        await _ctx.SaveChangesAsync(ct);

        _logger.LogInformation("[Job] Cleaned up {Count} old notifications", old.Count);
    }
}
