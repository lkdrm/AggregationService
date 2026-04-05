using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AggregationService.Infrastructure.Middleware;

public class CorrelationIdMiddleware
{
    private readonly ILogger<CorrelationIdMiddleware> _logger;
    private readonly RequestDelegate _next;
    private const string CorrelationIdHeader = "X-Correlation-Id";

    public CorrelationIdMiddleware(ILogger<CorrelationIdMiddleware> logger, RequestDelegate next)
    {
        _logger = logger;
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue(CorrelationIdHeader, out var correlationId))
        {
            correlationId = Guid.NewGuid().ToString();
        }

        context.Response.Headers[CorrelationIdHeader] = correlationId;
        context.Items["CorrelationId"] = correlationId.ToString();
        using (_logger.BeginScope($"CorrelationId: {correlationId}"))
        {
            await _next(context);
        }
    }
}