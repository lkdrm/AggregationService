using AggregationService.Application.Interfaces;
using AggregationService.Domain.Models;

namespace AggregationService.Infrastructure.Clients;

public class ProductClient : IProductClient
{
    private readonly Dictionary<string, ProductItem> _products = new()
    {
        { "1", new ProductItem("1", "MacBook Pro 16", "https://example.com/macbook.jpg") },
        { "2", new ProductItem("2", "Logitech MX Master 3", "https://example.com/mouse.jpg") },
        { "3", new ProductItem("3", "Keychron K2", "https://example.com/keyboard.jpg") }
    };

    public Task<ProductItem> GetProductItemAsync(string productId)
    {
        if (_products.TryGetValue(productId, out var productItem))
        {
            return Task.FromResult(productItem);
        }

        throw new KeyNotFoundException($"Product with ID{productId} not found");
    }
}
