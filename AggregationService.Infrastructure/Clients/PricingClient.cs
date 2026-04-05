using AggregationService.Application.Interfaces;
using AggregationService.Domain.Models;
using Polly;

namespace AggregationService.Infrastructure.Clients;

/// <summary>
/// A mock implementation of <see cref="IPricingClient"/> that provides simulated pricing data
/// with Polly-backed retry logic to mimic real-world transient failures.
/// </summary>
public class PricingClient : IPricingClient
{
    /// <summary>
    /// In-memory store mapping product IDs to their corresponding price details.
    /// Each entry contains the price amount and currency code.
    /// </summary>
    private readonly Dictionary<string, PriceDetails> _prices = new()
    {
        { "1", new PriceDetails(2500m, "USD") },
        { "2", new PriceDetails(99.99m, "USD") },
        { "3", new PriceDetails(120m, "USD") },
        { "4", new PriceDetails(1800m, "USD") },
        { "5", new PriceDetails(350m, "USD") },
        { "6", new PriceDetails(599m, "USD") },
        { "7", new PriceDetails(400m, "USD") },
        { "8", new PriceDetails(1200m, "USD") },
        { "9", new PriceDetails(399m, "USD") },
        { "10", new PriceDetails(1400m, "USD") },
        { "11", new PriceDetails(150m, "USD") },
        { "12", new PriceDetails(130m, "USD") },
        { "13", new PriceDetails(429m, "USD") },
        { "14", new PriceDetails(170m, "USD") },
        { "15", new PriceDetails(100m, "USD") },
        { "16", new PriceDetails(350m, "USD") },
        { "17", new PriceDetails(550m, "USD") },
        { "18", new PriceDetails(1600m, "USD") }
    };

    /// <summary>
    /// Retrieves the price details for the specified product ID.
    /// Applies a Polly retry policy to handle transient failures, retrying up to 5 times
    /// with a 100 ms delay between attempts. A 25% simulated failure rate and a random
    /// processing delay are used to mimic an unreliable external pricing service.
    /// </summary>
    /// <param name="productId">The unique identifier of the product.</param>
    /// <returns>
    /// A <see cref="PriceDetails"/> instance if the product ID is found;
    /// otherwise, <see langword="null"/>.
    /// </returns>
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
