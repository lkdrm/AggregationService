using AggregationService.Domain.Models;

namespace AggregationService.Application.Interfaces;

public interface IPricingClient
{
    Task<PriceDetails?> GetPriceDetailsAsync(string productId);
}
