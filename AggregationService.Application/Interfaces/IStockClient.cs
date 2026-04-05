using AggregationService.Domain.Models;

namespace AggregationService.Application.Interfaces;

/// <summary>
/// Defines the contract for a client that provides stock availability information for products.
/// </summary>
public interface IStockClient
{
    /// <summary>
    /// Retrieves the stock details for the specified product.
    /// </summary>
    /// <param name="productId">The unique identifier of the product.</param>
    /// <returns>
    /// A <see cref="StockDetails"/> instance if stock data is available;
    /// otherwise, <see langword="null"/>.
    /// </returns>
    Task<StockDetails?> GetStockDetailsAsync(string productId);
}
