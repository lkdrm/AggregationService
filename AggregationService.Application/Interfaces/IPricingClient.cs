using AggregationService.Domain.Models;

namespace AggregationService.Application.Interfaces;

/// <summary>
/// Defines the contract for a client that provides pricing information for products.
/// </summary>
public interface IPricingClient
{
    /// <summary>
    /// Retrieves the price details for the specified product.
    /// </summary>
    /// <param name="productId">The unique identifier of the product.</param>
    /// <returns>
    /// A <see cref="PriceDetails"/> instance if pricing data is available;
    /// otherwise, <see langword="null"/>.
    /// </returns>
    Task<PriceDetails?> GetPriceDetailsAsync(string productId);
}
