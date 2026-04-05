using AggregationService.Domain.Models;

namespace AggregationService.Application.Interfaces;

/// <summary>
/// Defines the contract for a repository that provides read access to aggregated product data.
/// </summary>
public interface IProductRepository
{
    /// <summary>
    /// Retrieves an aggregated product by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the product to retrieve.</param>
    /// <param name="cancellationToken">Token used to cancel the asynchronous operation.</param>
    /// <returns>
    /// An <see cref="AggregatedProduct"/> if found; otherwise, <see langword="null"/>.
    /// </returns>
    Task<AggregatedProduct?> GetByIdAsync(string id, CancellationToken cancellationToken);
}
