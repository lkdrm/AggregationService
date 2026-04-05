using AggregationService.Infrastructure.Clients;

namespace AggregationService.Tests.Clients;

/// <summary>
/// Unit tests for <see cref="StockClient"/> verifying correct stock detail lookups
/// for both known and unknown product IDs.
/// </summary>
public class StockClientTests
{
    private readonly StockClient _client = new();

    [Theory]
    [InlineData("1",  2,  true)]
    [InlineData("2",  0,  false)]
    [InlineData("5",  15, true)]
    [InlineData("14", 30, true)]
    public async Task GetStockDetailsAsync_KnownId_ReturnsCorrectDetails(
        string id, int expectedQuantity, bool expectedAvailability)
    {
        // Act
        var result = await _client.GetStockDetailsAsync(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedQuantity, result.Quantity);
        Assert.Equal(expectedAvailability, result.IsAvailable);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("19")]
    [InlineData("abc")]
    public async Task GetStockDetailsAsync_UnknownId_ReturnsNull(string id)
    {
        // Act
        var result = await _client.GetStockDetailsAsync(id);

        // Assert
        Assert.Null(result);
    }

    [Theory]
    [InlineData("2")]
    [InlineData("6")]
    [InlineData("11")]
    [InlineData("16")]
    public async Task GetStockDetailsAsync_OutOfStockProducts_HaveZeroQuantityAndIsAvailableFalse(string id)
    {
        // Act
        var result = await _client.GetStockDetailsAsync(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.Quantity);
        Assert.False(result.IsAvailable);
    }
}
