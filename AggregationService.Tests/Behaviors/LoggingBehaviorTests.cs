using AggregationService.Application.Behaviors;
using MediatR;
using Microsoft.Extensions.Logging.Abstractions;

namespace AggregationService.Tests.Behaviors;

/// <summary>
/// Unit tests for <see cref="LoggingBehavior{TRequest, TResponse}"/> verifying that
/// the next delegate is correctly invoked and exceptions are propagated.
/// </summary>
public class LoggingBehaviorTests
{
    private readonly LoggingBehavior<TestRequest, TestResponse> _behavior;

    public LoggingBehaviorTests()
    {
        _behavior = new LoggingBehavior<TestRequest, TestResponse>(
            NullLogger<LoggingBehavior<TestRequest, TestResponse>>.Instance);
    }

    [Fact]
    public async Task Handle_CallsNextDelegate_AndReturnsResult()
    {
        // Arrange
        var expected = new TestResponse("result");
        RequestHandlerDelegate<TestResponse> next = _ => Task.FromResult(expected);

        // Act
        var result = await _behavior.Handle(new TestRequest(), next, CancellationToken.None);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task Handle_NextThrows_PropagatesException()
    {
        // Arrange
        RequestHandlerDelegate<TestResponse> next = _ => throw new InvalidOperationException("boom");

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _behavior.Handle(new TestRequest(), next, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_PassesCancellationTokenToNext()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        CancellationToken captured = default;
        RequestHandlerDelegate<TestResponse> next = ct =>
        {
            captured = ct;
            return Task.FromResult(new TestResponse("ok"));
        };

        // Act
        await _behavior.Handle(new TestRequest(), next, cts.Token);

        // Assert
        Assert.Equal(cts.Token, captured);
    }

    private record TestRequest : IRequest<TestResponse>;
    private record TestResponse(string Value);
}
