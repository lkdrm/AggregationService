using AggregationService.Application.Interfaces;
using AggregationService.Domain.Models;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace AggregationService.Application.Queries;

/// <summary>
/// Handles <see cref="GetAggregatedProductQuery"/> by fetching the aggregated product
/// from the SQL read model via <see cref="IProductRepository"/>.
/// </summary>
public class GetAggregatedProductHandler : IRequestHandler<GetAggregatedProductQuery, AggregatedProduct>
{
    private readonly ILogger<GetAggregatedProductHandler> _logger;
    private readonly IProductRepository _productRepository;
    private readonly IMemoryCache _memoryCache;

    /// <summary>
    /// Initializes a new instance of <see cref="GetAggregatedProductHandler"/>.
    /// </summary>
    /// <param name="logger">Logger used for diagnostic output.</param>
    /// <param name="productRepository">The repository used to query aggregated product data.</param>
    public GetAggregatedProductHandler(ILogger<GetAggregatedProductHandler> logger, IProductRepository productRepository, IMemoryCache memoryCache)
    {
        _logger = logger;
        _productRepository = productRepository;
        _memoryCache = memoryCache;
    }

    /// <summary>
    /// Handles the query by retrieving the aggregated product from the repository.
    /// </summary>
    /// <param name="request">The query containing the product ID to look up.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>The <see cref="AggregatedProduct"/> for the requested ID.</returns>
    /// <exception cref="KeyNotFoundException">
    /// Thrown when no product with the specified ID exists in the read model.
    /// </exception>
    async Task<AggregatedProduct> IRequestHandler<GetAggregatedProductQuery, AggregatedProduct>.Handle(GetAggregatedProductQuery request, CancellationToken cancellationToken)
    {
        var cache = $"product_{request.Id}";
        if (_memoryCache.TryGetValue(cache, out AggregatedProduct? memory))
        {
            _logger.LogInformation("Memory cache hit for product: {ProductId}", request.Id);
            return memory!;
        }

        _logger.LogInformation("Fetching product {ProductId} from SQL Model", request.Id);

        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);

        if (product == null)
        {

            throw new KeyNotFoundException($"Product with id {request.Id}, not found.");
        }

        _memoryCache.Set(cache, product, TimeSpan.FromSeconds(30));

        return product;
    }
}
