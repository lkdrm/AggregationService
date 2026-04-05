using AggregationService.Application.Interfaces;
using AggregationService.Domain.Models;

namespace AggregationService.Infrastructure.Clients;

public class ProductClient : IProductClient
{
    private readonly Dictionary<string, ProductItem> _products = new()
    {
        { "1", new ProductItem("1", "MacBook Pro 16", "https://example.com/macbook.jpg") },
        { "2", new ProductItem("2", "Logitech MX Master 3", "https://example.com/mouse.jpg") },
        { "3", new ProductItem("3", "Keychron K2", "https://example.com/keyboard.jpg") },
        { "4", new ProductItem("4", "Dell XPS 15", "https://example.com/dell-xps.jpg") },
        { "5", new ProductItem("5", "Sony WH-1000XM5", "https://example.com/sony-headphones.jpg") },
        { "6", new ProductItem("6", "iPad Air M2", "https://example.com/ipad-air.jpg") },
        { "7", new ProductItem("7", "LG UltraGear 27\"", "https://example.com/lg-monitor.jpg") },
        { "8", new ProductItem("8", "Samsung Galaxy S24 Ultra", "https://example.com/s24-ultra.jpg") },
        { "9", new ProductItem("9", "Apple Watch Series 9", "https://example.com/apple-watch.jpg") },
        { "10", new ProductItem("10", "Asus ROG Zephyrus G14", "https://example.com/rog-laptop.jpg") },
        { "11", new ProductItem("11", "Razer DeathAdder V3 Pro", "https://example.com/razer-mouse.jpg") },
        { "12", new ProductItem("12", "NuPhy Halo75", "https://example.com/nuphy-keyboard.jpg") },
        { "13", new ProductItem("13", "Bose QuietComfort Ultra", "https://example.com/bose-headphones.jpg") },
        { "14", new ProductItem("14", "Samsung 990 PRO 2TB SSD", "https://example.com/samsung-ssd.jpg") },
        { "15", new ProductItem("15", "Anker 737 Power Bank", "https://example.com/anker-powerbank.jpg") },
        { "16", new ProductItem("16", "Nintendo Switch OLED", "https://example.com/switch.jpg") },
        { "17", new ProductItem("17", "Secretlab Titan Evo", "https://example.com/secretlab-chair.jpg") },
        { "18", new ProductItem("18", "Herman Miller Aeron", "https://example.com/aeron-chair.jpg") }
    };

    public Task<ProductItem> GetProductItemAsync(string productId)
    {
        if (_products.TryGetValue(productId, out var productItem))
        {
            return Task.FromResult(productItem);
        }

        throw new KeyNotFoundException($"Product with ID{productId} not found");
    }

    public int GetProductsCount() => _products.Count;
}
