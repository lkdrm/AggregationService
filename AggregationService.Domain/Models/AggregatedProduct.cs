namespace AggregationService.Domain.Models;

/// <summary>
/// Represents the fully aggregated view of a product combining
/// core product data, pricing, and stock information.
/// Returned to the API consumer as the query result.
/// </summary>
/// <param name="Id">The unique identifier of the product.</param>
/// <param name="Name">The display name of the product.</param>
/// <param name="ImageUrl">The URL pointing to the product image.</param>
/// <param name="Price">The pricing details, or <see langword="null"/> if unavailable.</param>
/// <param name="Stock">The stock details, or <see langword="null"/> if unavailable.</param>
public record AggregatedProduct(string Id, string Name, string ImageUrl, PriceDetails? Price, StockDetails? Stock);
