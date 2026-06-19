using System.Security.Cryptography;
using System.Text;

namespace Skeleton.Middleware;

/// <summary>
/// Adds ETag support and caching hints for GET responses.
/// Works alongside Redis cache to further reduce bandwidth.
/// </summary>
public class PerformanceMiddleware
{
    private readonly RequestDelegate _next;

    public PerformanceMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        // Only process GET requests
        if (!context.Request.Method.Equals("GET", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        // Capture response body
        var originalBody = context.Response.Body;
        using var buffer = new MemoryStream();
        context.Response.Body = buffer;

        await _next(context);

        buffer.Seek(0, SeekOrigin.Begin);
        var bodyBytes = buffer.ToArray();

        // Generate ETag from response body hash
        var etag = $"\"{ComputeHash(bodyBytes)}\"";
        context.Response.Headers["ETag"] = etag;

        // Add cache hints for public, non-sensitive endpoints
        if (!context.Response.Headers.ContainsKey("Cache-Control"))
        {
            var path = context.Request.Path.Value ?? "";
            if (path.StartsWith("/api/products") || path.StartsWith("/api/categories"))
                context.Response.Headers["Cache-Control"] = "public, max-age=60";
            else
                context.Response.Headers["Cache-Control"] = "no-cache";
        }

        // Check If-None-Match (conditional GET)
        var clientEtag = context.Request.Headers["If-None-Match"].FirstOrDefault();
        if (clientEtag == etag && context.Response.StatusCode == 200)
        {
            context.Response.StatusCode = 304;
            context.Response.Body = originalBody;
            return;
        }

        context.Response.Body = originalBody;
        if (bodyBytes.Length > 0)
            await context.Response.Body.WriteAsync(bodyBytes, CancellationToken.None);
    }

    private static string ComputeHash(byte[] data)
    {
        var hash  = MD5.HashData(data);
        return Convert.ToHexString(hash).ToLowerInvariant()[..16];
    }
}
