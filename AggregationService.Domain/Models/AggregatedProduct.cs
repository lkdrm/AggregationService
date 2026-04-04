namespace AggregationService.Domain.Models;

public record AggregatedProduct(string Id, string Name, string ImageUrl, PriceDetails? Price, StockDetails? Stock);
