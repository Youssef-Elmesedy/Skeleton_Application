namespace Skeleton.Middleware;

/// <summary>
/// Reads Accept-Language header and sets the current thread culture.
/// Supports: en (default), ar
/// </summary>
public class LocalizationMiddleware
{
    private readonly RequestDelegate _next;
    private static readonly HashSet<string> _supported = new(StringComparer.OrdinalIgnoreCase)
        { "en", "ar", "en-US", "ar-EG", "ar-SA" };

    public LocalizationMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var lang = context.Request.Headers["Accept-Language"].FirstOrDefault()
                ?? context.Request.Query["lang"].FirstOrDefault()
                ?? "en";

        // Normalize: "ar-EG" → "ar"
        var culture = lang.Split(',')[0].Trim();
        if (!_supported.Contains(culture))
            culture = "en";

        var cultureInfo = new System.Globalization.CultureInfo(culture);
        System.Threading.Thread.CurrentThread.CurrentCulture   = cultureInfo;
        System.Threading.Thread.CurrentThread.CurrentUICulture = cultureInfo;

        context.Items["Culture"] = culture;
        context.Response.Headers["Content-Language"] = culture;

        await _next(context);
    }
}
