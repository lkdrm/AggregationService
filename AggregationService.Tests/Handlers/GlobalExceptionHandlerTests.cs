using AggregationService.Infrastructure.Handlers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;

namespace AggregationService.Tests.Handlers;

/// <summary>
/// Unit tests for <see cref="GlobalExceptionHandler"/> verifying correct HTTP status codes,
/// response body content, and correlation ID propagation.
/// </summary>
public class GlobalExceptionHandlerTests
{
    private readonly GlobalExceptionHandler _handler =
        new(NullLogger<GlobalExceptionHandler>.Instance);

    [Fact]
    public async Task TryHandleAsync_KeyNotFoundException_ReturnsTrue_And404()
    {
        // Arrange
        var context = CreateContext();

        // Act
        var handled = await _handler.TryHandleAsync(
            context, new KeyNotFoundException("not found"), CancellationToken.None);

        // Assert
        Assert.True(handled);
        Assert.Equal(StatusCodes.Status404NotFound, context.Response.StatusCode);
    }

    [Fact]
    public async Task TryHandleAsync_GenericException_ReturnsTrue_And500()
    {
        // Arrange
        var context = CreateContext();

        // Act
        var handled = await _handler.TryHandleAsync(
            context, new Exception("error"), CancellationToken.None);

        // Assert
        Assert.True(handled);
        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
    }

    [Fact]
    public async Task TryHandleAsync_WithCorrelationId_IncludesItInResponseBody()
    {
        // Arrange
        const string correlationId = "test-correlation-123";
        var context = CreateContext();
        context.Items["CorrelationId"] = correlationId;

        // Act
        await _handler.TryHandleAsync(context, new Exception("boom"), CancellationToken.None);

        // Assert
        var body = await ReadBodyAsync(context);
        Assert.Contains(correlationId, body);
    }

    [Fact]
    public async Task TryHandleAsync_ExceptionMessage_IsIncludedInResponseBody()
    {
        // Arrange
        const string message = "Product with id 999, not found.";
        var context = CreateContext();

        // Act
        await _handler.TryHandleAsync(context, new KeyNotFoundException(message), CancellationToken.None);

        // Assert
        var body = await ReadBodyAsync(context);
        Assert.Contains(message, body);
    }

    [Fact]
    public async Task TryHandleAsync_InvalidOperationException_Returns500WithInternalServerErrorTitle()
    {
        // Arrange
        var context = CreateContext();

        // Act
        await _handler.TryHandleAsync(
            context, new InvalidOperationException("something broke"), CancellationToken.None);

        // Assert
        var body = await ReadBodyAsync(context);
        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
        Assert.Contains("Internal Server Error", body);
    }

    [Fact]
    public async Task TryHandleAsync_KeyNotFoundException_ResponseBodyContains404Title()
    {
        // Arrange
        var context = CreateContext();

        // Act
        await _handler.TryHandleAsync(
            context, new KeyNotFoundException("missing"), CancellationToken.None);

        // Assert
        var body = await ReadBodyAsync(context);
        Assert.Contains("Resource Not Found", body);
    }

    private static DefaultHttpContext CreateContext()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        return context;
    }

    private static async Task<string> ReadBodyAsync(DefaultHttpContext context)
    {
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        return await new StreamReader(context.Response.Body).ReadToEndAsync();
    }
}
