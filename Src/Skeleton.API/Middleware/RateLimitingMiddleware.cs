using System.Collections.Concurrent;

namespace Skeleton.Middleware;

/// <summary>
/// Custom sliding-window rate limiter middleware.
/// Tracks requests per IP using an in-memory concurrent dictionary.
/// Complements the built-in ASP.NET Core rate limiter (which handles named policies).
/// </summary>
public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RateLimitingMiddleware> _logger;
    private readonly IConfiguration _config;

    // IP → list of request timestamps
    private static readonly ConcurrentDictionary<string, Queue<DateTime>> _requests = new();
    private static readonly object _lock = new();

    public RateLimitingMiddleware(
        RequestDelegate next,
        ILogger<RateLimitingMiddleware> logger,
        IConfiguration config)
    {
        _next = next;
        _logger = logger;
        _config = config;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var limit = int.Parse(_config["RateLimiting:RequestsPerWindow"] ?? "100");
        var window = TimeSpan.FromSeconds(int.Parse(_config["RateLimiting:WindowSeconds"] ?? "60"));

        var now = DateTime.UtcNow;
        var cutoff = now - window;

        Queue<DateTime> queue;

        lock (_lock)
        {
            queue = _requests.GetOrAdd(ip, _ => new Queue<DateTime>());

            while (queue.Count > 0 && queue.Peek() < cutoff)
                queue.Dequeue();

            if (queue.Count >= limit)
            {
                var retryAfter = (int)(window - (now - queue.Peek())).TotalSeconds + 1;

                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.Response.Headers["Retry-After"] = retryAfter.ToString();

                var response = new ApiResponse<object>
                {
                    IsSuccess = false,
                    ErrorCode = "RateLimit",
                    Message = $"Too many requests. Retry after {retryAfter}s"
                };

                context.Response.WriteAsJsonAsync(response).GetAwaiter().GetResult();
                return;
            }

            queue.Enqueue(now);
        }

        await _next(context);

        queue.Enqueue(now);

        // Add rate limit info headers to all responses
        context.Response.Headers["X-RateLimit-Limit"] = limit.ToString();
        context.Response.Headers["X-RateLimit-Remaining"] = (limit - queue.Count).ToString();
    }

}
