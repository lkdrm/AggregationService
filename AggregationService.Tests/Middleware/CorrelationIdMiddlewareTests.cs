using AggregationService.Infrastructure.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;

namespace AggregationService.Tests.Middleware;

/// <summary>
/// Unit tests for <see cref="CorrelationIdMiddleware"/> verifying correlation ID
/// generation, propagation, and context storage behaviour.
/// </summary>
public class CorrelationIdMiddlewareTests
{
    private const string HeaderName = "X-Correlation-Id";

    [Fact]
    public async Task InvokeAsync_NoCorrelationIdHeader_GeneratesNewGuidAsCorrelationId()
    {
        // Arrange
        var context = CreateContext();

        // Act
        await CreateMiddleware().InvokeAsync(context);

        // Assert
        Assert.True(context.Response.Headers.ContainsKey(HeaderName));
        var id = context.Response.Headers[HeaderName].ToString();
        Assert.NotEmpty(id);
        Assert.True(Guid.TryParse(id, out _));
    }

    [Fact]
    public async Task InvokeAsync_WithExistingCorrelationIdHeader_PreservesProvidedId()
    {
        // Arrange
        const string existingId = "existing-id-abc";
        var context = CreateContext();
        context.Request.Headers[HeaderName] = existingId;

        // Act
        await CreateMiddleware().InvokeAsync(context);

        // Assert
        Assert.Equal(existingId, context.Response.Headers[HeaderName].ToString());
    }

    [Fact]
    public async Task InvokeAsync_SetsCorrelationIdInContextItems()
    {
        // Arrange
        const string correlationId = "item-id-xyz";
        var context = CreateContext();
        context.Request.Headers[HeaderName] = correlationId;

        // Act
        await CreateMiddleware().InvokeAsync(context);

        // Assert
        Assert.True(context.Items.ContainsKey("CorrelationId"));
        Assert.Equal(correlationId, context.Items["CorrelationId"]);
    }

    [Fact]
    public async Task InvokeAsync_AlwaysCallsNextMiddleware()
    {
        // Arrange
        var nextCalled = false;
        var context = CreateContext();
        var middleware = new CorrelationIdMiddleware(
            NullLogger<CorrelationIdMiddleware>.Instance,
            _ => { nextCalled = true; return Task.CompletedTask; });

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        Assert.True(nextCalled);
    }

    [Fact]
    public async Task InvokeAsync_GeneratedId_IsAlsoStoredInContextItems()
    {
        // Arrange
        var context = CreateContext();

        // Act
        await CreateMiddleware().InvokeAsync(context);

        // Assert
        var responseHeaderId = context.Response.Headers[HeaderName].ToString();
        var contextItemId = context.Items["CorrelationId"]?.ToString();
        Assert.Equal(responseHeaderId, contextItemId);
    }

    private static DefaultHttpContext CreateContext()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        return context;
    }

    private static CorrelationIdMiddleware CreateMiddleware() =>
        new(NullLogger<CorrelationIdMiddleware>.Instance, _ => Task.CompletedTask);
}
