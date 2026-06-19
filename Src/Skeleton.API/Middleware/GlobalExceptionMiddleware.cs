using Skeleton.Domain.Eunm;
using Skeleton.Domain.Exceptions;

namespace Skeleton.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        IWebHostEnvironment environment,
        ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _environment = environment;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        int statusCode;
        string errorCode;
        string message;
        object? errors = null;

        switch (exception)
        {
            // ── FluentValidation ─────────────────────────────────
            case FluentValidation.ValidationException valEx:
                statusCode = StatusCodes.Status400BadRequest;
                errorCode = "Validation Error";
                message = "Validation failed for one or more fields.";
                errors = valEx.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray());
                break;

            // ── Domain BusinessException ──────────────────────────
            case BusinessException bizEx:
                (statusCode, errorCode) = bizEx.ErroType switch
                {
                    ErrorType.NotFound => (StatusCodes.Status404NotFound, "Not Found"),
                    ErrorType.Validation => (StatusCodes.Status400BadRequest, "Validation Error"),
                    ErrorType.Conflict => (StatusCodes.Status409Conflict, "Conflict Error"),
                    _ => (StatusCodes.Status400BadRequest, "Business Error")
                };
                message = bizEx.Message;
                break;

            // ── Unauthorized ──────────────────────────────────────
            case UnauthorizedAccessException:
                statusCode = StatusCodes.Status401Unauthorized;
                errorCode = "Unauthorized";
                message = "Authentication is required.";
                break;

            // ── Rate Limit (429) — handled by ASP.NET middleware,
            //    but fallback here just in case
            case InvalidOperationException ioe when ioe.Message.Contains("rate limit"):
                statusCode = StatusCodes.Status429TooManyRequests;
                errorCode = "Rate Limit Exceeded";
                message = "Too many requests. Please try again later.";
                break;

            // ── Unhandled ─────────────────────────────────────────
            default:
                statusCode = StatusCodes.Status500InternalServerError;
                errorCode = "Server Error";
                message = "An unexpected error occurred.";
                break;
        }

        var response = new ApiResponse<object>
        {
            IsSuccess = false,
            ErrorCode = errorCode,
            Message = message,
            Errors = errors
        };

        // In development, attach stack trace
        if (_environment.IsDevelopment() && statusCode == 500)
        {
            response.Errors = new Dictionary<string, string[]>
            {
                ["exception"] = new[] { exception.GetType().Name },
                ["message"] = new[] { exception.Message },
                ["stackTrace"] = new[] { exception.StackTrace ?? "N/A" }
            };
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        await context.Response.WriteAsJsonAsync(response);
    }
}
