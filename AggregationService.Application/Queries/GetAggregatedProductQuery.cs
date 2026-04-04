using AggregationService.Domain.Models;
using MediatR;

namespace AggregationService.Application.Queries;

public record GetAggregatedProductQuery(string Id) : IRequest<AggregatedProduct>;
