using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Skeleton.Infrastructure.HealthChecks;

public class RedisHealthCheck : IHealthCheck
{
    private readonly IDistributedCache _cache;

    public RedisHealthCheck(IDistributedCache cache) => _cache = cache;

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken ct = default)
    {
        try
        {
            var key = $"healthcheck:{Guid.NewGuid()}";
            await _cache.SetStringAsync(key, "ok",
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(5) }, ct);
            await _cache.RemoveAsync(key, ct);
            return HealthCheckResult.Healthy("Redis is reachable.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Degraded("Redis is unreachable — memory cache is active.", ex);
        }
    }
}
