namespace AggregationService.Domain.Models;

/// <summary>
/// Represents the pricing information for a product.
/// </summary>
/// <param name="Amount">The monetary value of the product price.</param>
/// <param name="Currency">The ISO 4217 currency code (e.g., "USD").</param>
public record PriceDetails(decimal Amount, string Currency);