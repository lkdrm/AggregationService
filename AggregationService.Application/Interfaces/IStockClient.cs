using AggregationService.Domain.Models;

namespace AggregationService.Application.Interfaces;

public interface IStockClient
{
    Task<StockDetails?> GetStockDetailsAsync(string productId);
}
