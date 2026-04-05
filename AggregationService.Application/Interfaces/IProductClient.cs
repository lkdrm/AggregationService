using AggregationService.Domain.Models;

namespace AggregationService.Application.Interfaces;

/// <summary>
/// Defines the contract for a client that provides access to the product catalog.
/// </summary>
public interface IProductClient
{
    /// <summary>
    /// Retrieves a product item by its unique identifier.
    /// </summary>
    /// <param name="productId">The unique identifier of the product.</param>
    /// <returns>A <see cref="Task{ProductItem}"/> containing the matching product.</returns>
    Task<ProductItem> GetProductItemAsync(string productId);

    /// <summary>
    /// Returns the total number of products available in the catalog.
    /// </summary>
    /// <returns>The count of available products.</returns>
    int GetProductsCount();
}
