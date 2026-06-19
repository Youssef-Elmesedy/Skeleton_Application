namespace Skeleton.Application.Services.Interfaces;

public interface ICacheService
{
    // ── Get ──────────────────────────────────────────────────────
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);

    // ── Set ──────────────────────────────────────────────────────
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken cancellationToken = default);

    // ── Remove ───────────────────────────────────────────────────
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);

    // ── Remove by pattern ─────────────────────────────────────────
    Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default);

    // ── Get or Set ───────────────────────────────────────────────
    Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiry = null, CancellationToken cancellationToken = default);
}
