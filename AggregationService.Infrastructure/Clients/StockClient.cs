using AggregationService.Application.Interfaces;
using AggregationService.Domain.Models;

namespace AggregationService.Infrastructure.Clients;

/// <summary>
/// A mock implementation of <see cref="IStockClient"/> that provides
/// in-memory stock details for a predefined set of products.
/// </summary>
public class StockClient : IStockClient
{
    /// <summary>
    /// In-memory store mapping product IDs to their corresponding stock details.
    /// Each entry contains the available quantity and whether the product is in stock.
    /// </summary>
    private readonly Dictionary<string, StockDetails> _stockDetails = new()
    {
        {"1", new StockDetails(2, true) },
        {"2", new StockDetails(0, false) },
        {"3", new StockDetails(5, true) },
        {"4", new StockDetails(10, true) },
        {"5", new StockDetails(15, true) },
        {"6", new StockDetails(0, false) },
        {"7", new StockDetails(8, true) },
        {"8", new StockDetails(25, true) },
        {"9", new StockDetails(5, true) },
        {"10", new StockDetails(3, true) },
        {"11", new StockDetails(0, false) },
        {"12", new StockDetails(12, true) },
        {"13", new StockDetails(7, true) },
        {"14", new StockDetails(30, true) },
        {"15", new StockDetails(50, true) },
        {"16", new StockDetails(0, false) },
        {"17", new StockDetails(4, true) },
        {"18", new StockDetails(2, true) }
    };

    /// <summary>
    /// Retrieves the stock details for the specified product ID.
    /// </summary>
    /// <param name="productId">The unique identifier of the product.</param>
    /// <returns>
    /// A <see cref="StockDetails"/> instance if the product ID is found;
    /// otherwise, <see langword="null"/>.
    /// </returns>
    public Task<StockDetails?> GetStockDetailsAsync(string productId)
    {
        if (_stockDetails.TryGetValue(productId, out var stockDetails))
        {
            return Task.FromResult(stockDetails);
        }

        return Task.FromResult<StockDetails?>(null);
    }
}
