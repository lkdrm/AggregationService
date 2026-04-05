using AggregationService.Infrastructure.Clients;

namespace AggregationService.Tests.Clients;

/// <summary>
/// Unit tests for <see cref="ProductClient"/> verifying product retrieval,
/// unknown ID handling, and catalog count.
/// </summary>
public class ProductClientTests
{
    private readonly ProductClient _client = new();

    [Theory]
    [InlineData("1",  "MacBook Pro 16",          "https://example.com/macbook.jpg")]
    [InlineData("5",  "Sony WH-1000XM5",         "https://example.com/sony-headphones.jpg")]
    [InlineData("10", "Asus ROG Zephyrus G14",   "https://example.com/rog-laptop.jpg")]
    [InlineData("18", "Herman Miller Aeron",      "https://example.com/aeron-chair.jpg")]
    public async Task GetProductItemAsync_KnownId_ReturnsCorrectProduct(
        string id, string expectedName, string expectedImageUrl)
    {
        // Act
        var result = await _client.GetProductItemAsync(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
        Assert.Equal(expectedName, result.Name);
        Assert.Equal(expectedImageUrl, result.ImageUrl);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("19")]
    [InlineData("abc")]
    [InlineData("")]
    public async Task GetProductItemAsync_UnknownId_ThrowsKeyNotFoundException(string id)
    {
        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _client.GetProductItemAsync(id));
    }

    [Fact]
    public void GetProductsCount_Returns18()
    {
        // Act & Assert
        Assert.Equal(18, _client.GetProductsCount());
    }
}
