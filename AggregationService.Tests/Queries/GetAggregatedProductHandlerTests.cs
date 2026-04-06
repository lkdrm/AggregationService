using AggregationService.Application.Interfaces;
using AggregationService.Application.Queries;
using AggregationService.Domain.Models;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace AggregationService.Tests.Queries;

/// <summary>
/// Unit tests for <see cref="GetAggregatedProductHandler"/> verifying correct product
/// retrieval and exception behaviour when the product is not found.
/// </summary>
public class GetAggregatedProductHandlerTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<IMemoryCache> _memoryCacheMock;
    private readonly IRequestHandler<GetAggregatedProductQuery, AggregatedProduct> _handler;

    public GetAggregatedProductHandlerTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _memoryCacheMock = new Mock<IMemoryCache>();

        // Configure mock to handle Set extension method gracefully
        _memoryCacheMock.DefaultValue = DefaultValue.Mock;

        _handler = new GetAggregatedProductHandler(
            NullLogger<GetAggregatedProductHandler>.Instance,
            _productRepositoryMock.Object,
            _memoryCacheMock.Object);
    }

    [Fact]
    public async Task Handle_ProductExists_ReturnsAggregatedProduct()
    {
        // Arrange
        const string productId = "product-123";
        var expectedProduct = new AggregatedProduct(
            productId,
            "Test Product",
            "https://example.com/image.jpg",
            new PriceDetails(9.99m, "USD"),
            new StockDetails(42, true));

        var cacheEntry = new Mock<ICacheEntry>();
        cacheEntry.SetupProperty(x => x.AbsoluteExpirationRelativeToNow);
        cacheEntry.SetupProperty(x => x.Value);

        _memoryCacheMock
            .Setup(c => c.TryGetValue(It.IsAny<object>(), out It.Ref<object?>.IsAny))
            .Returns(false);

        _memoryCacheMock
            .Setup(c => c.CreateEntry(It.IsAny<object>()))
            .Returns(cacheEntry.Object);

        _productRepositoryMock
            .Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedProduct);

        var query = new GetAggregatedProductQuery(productId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedProduct.Id, result.Id);
        Assert.Equal(expectedProduct.Name, result.Name);
        Assert.Equal(expectedProduct.ImageUrl, result.ImageUrl);
        Assert.Equal(expectedProduct.Price, result.Price);
        Assert.Equal(expectedProduct.Stock, result.Stock);
    }

    [Fact]
    public async Task Handle_ProductNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        const string productId = "non-existent-id";

        var cacheEntry = new Mock<ICacheEntry>();
        cacheEntry.SetupProperty(x => x.AbsoluteExpirationRelativeToNow);
        cacheEntry.SetupProperty(x => x.Value);

        _memoryCacheMock
            .Setup(c => c.TryGetValue(It.IsAny<object>(), out It.Ref<object?>.IsAny))
            .Returns(false);

        _memoryCacheMock
            .Setup(c => c.CreateEntry(It.IsAny<object>()))
            .Returns(cacheEntry.Object);

        _productRepositoryMock
            .Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((AggregatedProduct?)null);

        var query = new GetAggregatedProductQuery(productId);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _handler.Handle(query, CancellationToken.None));

        Assert.Contains(productId, exception.Message);
    }
}
