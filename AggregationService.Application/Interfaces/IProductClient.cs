using AggregationService.Domain.Models;

namespace AggregationService.Application.Interfaces;

public interface IProductClient
{
    Task<ProductItem> GetProductItemAsync(string productId);
}
