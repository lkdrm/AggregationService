using AggregationService.Application.Interfaces;
using AggregationService.Domain.Models;
using Polly;

namespace AggregationService.Infrastructure.Clients;

public class PricingClient : IPricingClient
{
    private readonly Dictionary<string, PriceDetails> _prices = new()
    {
        { "1", new PriceDetails(2500m, "USD") },
        { "2", new PriceDetails(99.99m, "USD") },
        { "3", new PriceDetails(120m, "USD") }
    };

    public async Task<PriceDetails?> GetPriceDetailsAsync(string productId)
    {
        var retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromMilliseconds(100),
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    Console.WriteLine($"[Polly] Retry {retryCount} for PricingClient due to: {exception.Message}");
                });

        return await retryPolicy.ExecuteAsync(async () =>
        {
            await Task.Delay(Random.Shared.Next(500, 801));

            if (Random.Shared.Next(1, 101) <= 25)
            {
                throw new Exception("Pricing service failure!");
            }

            if (_prices.TryGetValue(productId, out var priceDetails))
            {
                return priceDetails;
            }

            return null;
        });
    }
}
