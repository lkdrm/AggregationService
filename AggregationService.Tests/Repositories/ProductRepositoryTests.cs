using AggregationService.Domain.Models;
using AggregationService.Sql;
using AggregationService.Sql.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AggregationService.Tests.Repositories;

/// <summary>
/// Unit tests for <see cref="ProductRepository"/> using an isolated in-memory EF Core database.
/// </summary>
public class ProductRepositoryTests : IDisposable
{
    private readonly AppDbContext _dbContext;
    private readonly ProductRepository _repository;

    public ProductRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new AppDbContext(options);
        _repository = new ProductRepository(_dbContext);
    }

    public void Dispose() => _dbContext.Dispose();

    [Fact]
    public async Task GetByIdAsync_ExistingProduct_ReturnsAggregatedProduct()
    {
        // Arrange
        await SeedProductAsync("1", "MacBook Pro 16", "https://example.com/macbook.jpg",
            new PriceDetails(2500m, "USD"), new StockDetails(2, true));

        // Act
        var result = await _repository.GetByIdAsync("1", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("1", result.Id);
        Assert.Equal("MacBook Pro 16", result.Name);
        Assert.Equal("https://example.com/macbook.jpg", result.ImageUrl);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingProduct_ReturnsNull()
    {
        // Act
        var result = await _repository.GetByIdAsync("non-existent", CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_MapsAllPropertiesCorrectly()
    {
        // Arrange
        await SeedProductAsync("42", "Test Product", "https://example.com/test.jpg",
            new PriceDetails(99.99m, "EUR"), new StockDetails(10, true));

        // Act
        var result = await _repository.GetByIdAsync("42", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(99.99m, result.Price!.Amount);
        Assert.Equal("EUR", result.Price.Currency);
        Assert.Equal(10, result.Stock!.Quantity);
        Assert.True(result.Stock.IsAvailable);
    }

    [Fact]
    public async Task GetByIdAsync_OutOfStockProduct_IsAvailableIsFalse()
    {
        // Arrange
        await SeedProductAsync("2", "Logitech MX Master 3", "https://example.com/mouse.jpg",
            new PriceDetails(99.99m, "USD"), new StockDetails(0, false));

        // Act
        var result = await _repository.GetByIdAsync("2", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0, result.Stock!.Quantity);
        Assert.False(result.Stock.IsAvailable);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsOnlyRequestedProduct_WhenMultipleExist()
    {
        // Arrange
        await SeedProductAsync("1", "Product A", "https://example.com/a.jpg",
            new PriceDetails(10m, "USD"), new StockDetails(5, true));
        await SeedProductAsync("2", "Product B", "https://example.com/b.jpg",
            new PriceDetails(20m, "USD"), new StockDetails(3, true));

        // Act
        var result = await _repository.GetByIdAsync("2", CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("2", result.Id);
        Assert.Equal("Product B", result.Name);
    }

    private async Task SeedProductAsync(string id, string name, string imageUrl,
        PriceDetails price, StockDetails stock)
    {
        var readModel = new ProductReadModel
        {
            Id = id,
            Name = name,
            ImageUrl = imageUrl,
            PriceDetails = price,
            StockDetails = stock
        };
        await _dbContext.Products.AddAsync(readModel);
        await _dbContext.SaveChangesAsync();
    }
}
