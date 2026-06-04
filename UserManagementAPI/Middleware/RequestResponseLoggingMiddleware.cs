using System.Diagnostics;
using System.Text;

namespace UserManagementAPI.Middleware;

public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestResponseLoggingMiddleware> _logger;
    private const int MaxLogBodyLength = 2000;

    public RequestResponseLoggingMiddleware(RequestDelegate next, ILogger<RequestResponseLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var requestBody = await ReadRequestBodyAsync(context.Request);

        _logger.LogInformation(
            "Incoming request. TraceId: {TraceId}, Method: {Method}, Path: {Path}, Query: {Query}, Body: {Body}",
            context.TraceIdentifier,
            context.Request.Method,
            context.Request.Path,
            context.Request.QueryString.Value,
            requestBody);

        var originalResponseBody = context.Response.Body;
        await using var responseBodyMemoryStream = new MemoryStream();
        context.Response.Body = responseBodyMemoryStream;

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();

            responseBodyMemoryStream.Position = 0;
            var responseBody = await new StreamReader(responseBodyMemoryStream).ReadToEndAsync();
            responseBodyMemoryStream.Position = 0;

            _logger.LogInformation(
                "Outgoing response. TraceId: {TraceId}, Method: {Method}, Path: {Path}, StatusCode: {StatusCode}, DurationMs: {DurationMs}, Body: {Body}",
                context.TraceIdentifier,
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds,
                Truncate(responseBody));

            await responseBodyMemoryStream.CopyToAsync(originalResponseBody);
            context.Response.Body = originalResponseBody;
        }
    }

    private static async Task<string> ReadRequestBodyAsync(HttpRequest request)
    {
        request.EnableBuffering();

        using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        request.Body.Position = 0;

        return Truncate(body);
    }

    private static string Truncate(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        return value.Length <= MaxLogBodyLength
            ? value
            : value[..MaxLogBodyLength] + "...<truncated>";
    }
}
