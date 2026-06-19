using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Skeleton.Application.Services.Interfaces;

namespace Skeleton.Infrastructure.Services;

/// <summary>
/// Hybrid cache: tries Redis (IDistributedCache) first, falls back to IMemoryCache.
/// </summary>
public class CacheService : ICacheService
{
    private readonly IDistributedCache  _distributed;
    private readonly IMemoryCache       _memory;
    private readonly ILogger<CacheService> _logger;

    private static readonly TimeSpan DefaultExpiry = TimeSpan.FromMinutes(10);

    public CacheService(
        IDistributedCache     distributed,
        IMemoryCache          memory,
        ILogger<CacheService> logger)
    {
        _distributed = distributed;
        _memory      = memory;
        _logger      = logger;
    }

    // ─────────────────────────────────────────────────────────────
    //  GET
    // ─────────────────────────────────────────────────────────────
    public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        // 1. Try Redis
        try
        {
            var bytes = await _distributed.GetAsync(key, ct);
            if (bytes is not null)
                return JsonSerializer.Deserialize<T>(bytes);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis GET failed for key {Key}, falling back to memory", key);
        }

        // 2. Fallback: MemoryCache
        return _memory.TryGetValue(key, out T? cached) ? cached : default;
    }

    // ─────────────────────────────────────────────────────────────
    //  SET
    // ─────────────────────────────────────────────────────────────
    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken ct = default)
    {
        var ttl   = expiry ?? DefaultExpiry;
        var bytes = JsonSerializer.SerializeToUtf8Bytes(value);

        // 1. Memory Cache (always)
        _memory.Set(key, value, ttl);

        // 2. Redis (best-effort)
        try
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = ttl
            };
            await _distributed.SetAsync(key, bytes, options, ct);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis SET failed for key {Key}", key);
        }
    }

    // ─────────────────────────────────────────────────────────────
    //  REMOVE
    // ─────────────────────────────────────────────────────────────
    public async Task RemoveAsync(string key, CancellationToken ct = default)
    {
        _memory.Remove(key);

        try { await _distributed.RemoveAsync(key, ct); }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redis REMOVE failed for key {Key}", key);
        }
    }

    // ─────────────────────────────────────────────────────────────
    //  REMOVE by prefix (memory only — Redis would need SCAN)
    // ─────────────────────────────────────────────────────────────
    public Task RemoveByPrefixAsync(string prefix, CancellationToken ct = default)
    {
        // For MemoryCache with prefix tracking, we use a simple convention.
        // In production with Redis, use StackExchange.Redis SCAN + DEL.
        _logger.LogDebug("RemoveByPrefix: {Prefix}", prefix);
        return Task.CompletedTask;
    }

    // ─────────────────────────────────────────────────────────────
    //  GET OR SET
    // ─────────────────────────────────────────────────────────────
    public async Task<T> GetOrSetAsync<T>(
        string key, Func<Task<T>> factory,
        TimeSpan? expiry = null, CancellationToken ct = default)
    {
        var cached = await GetAsync<T>(key, ct);
        if (cached is not null) return cached;

        var value = await factory();
        await SetAsync(key, value, expiry, ct);
        return value;
    }
}
