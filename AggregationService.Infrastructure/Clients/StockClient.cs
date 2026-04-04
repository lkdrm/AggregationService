using AggregationService.Application.Interfaces;
using AggregationService.Domain.Models;

namespace AggregationService.Infrastructure.Clients;

public class StockClient : IStockClient
{
    private readonly HttpClient _httpClient;
    private readonly Dictionary<string, StockDetails> _stockDetails = new()
    {
        {"1", new StockDetails(2, true) },
        {"2", new StockDetails(0, false) },
        {"3", new StockDetails(5, true) },
        {"4", new StockDetails(9, true) },
        {"5", new StockDetails(0, false) },
    };

    public StockClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Task<StockDetails?> GetStockDetailsAsync(string productId)
    {
        if (_stockDetails.TryGetValue(productId, out var stockDetails))
        {
            return Task.FromResult(stockDetails);
        }

        return Task.FromResult<StockDetails?>(null);
    }
}
