using AggregationService.Domain.Models;

namespace AggregationService.Application.Interfaces;

public interface IProductRepository
{
    Task<AggregatedProduct?> GetByIdAsync(string id, CancellationToken cancellationToken);
}
