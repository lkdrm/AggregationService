namespace AggregationService.Domain.Models;

/// <summary>
/// Represents the stock availability information for a product.
/// </summary>
/// <param name="Quantity">The number of units currently available in stock.</param>
/// <param name="IsAvailable">Indicates whether the product is available for purchase.</param>
public record StockDetails(int Quantity, bool IsAvailable);
