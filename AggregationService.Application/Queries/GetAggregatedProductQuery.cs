using AggregationService.Domain.Models;
using MediatR;

namespace AggregationService.Application.Queries;

/// <summary>
/// MediatR query that requests the fully aggregated product data for a given product ID.
/// </summary>
/// <param name="Id">The unique identifier of the product to retrieve.</param>
public record GetAggregatedProductQuery(string Id) : IRequest<AggregatedProduct>;
