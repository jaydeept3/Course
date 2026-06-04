using System.Text.Json;

namespace UserManagementAPI.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
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
            _logger.LogError(ex, "Unhandled exception while processing {Method} {Path}", context.Request.Method, context.Request.Path);

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            var payload = new
            {
                errorCode = "INTERNAL_SERVER_ERROR",
                message = "An unexpected error occurred.",
                traceId = context.TraceIdentifier,
                timestampUtc = DateTime.UtcNow
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
        }
    }
}
