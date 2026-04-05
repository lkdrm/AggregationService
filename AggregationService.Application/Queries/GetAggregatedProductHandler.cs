using AggregationService.Application.Interfaces;
using AggregationService.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AggregationService.Application.Queries;

public class GetAggregatedProductHandler : IRequestHandler<GetAggregatedProductQuery, AggregatedProduct>
{
    private readonly ILogger<GetAggregatedProductHandler> _logger;
    private readonly IProductRepository _productRepository;

    public GetAggregatedProductHandler(ILogger<GetAggregatedProductHandler> logger, IProductRepository productRepository)
    {
        _logger = logger;
        _productRepository = productRepository;
    }

    async Task<AggregatedProduct> IRequestHandler<GetAggregatedProductQuery, AggregatedProduct>.Handle(GetAggregatedProductQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Fetching product {request.Id} from SQL Model");

        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken);

        return product ?? throw new KeyNotFoundException($"Product with id {request.Id}, not found.");
    }
}
