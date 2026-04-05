namespace AggregationService.Domain.Models;

/// <summary>
/// Represents core product information returned by the product catalog client.
/// </summary>
/// <param name="Id">The unique identifier of the product.</param>
/// <param name="Name">The display name of the product.</param>
/// <param name="ImageUrl">The URL pointing to the product image.</param>
public record ProductItem(string Id, string Name, string ImageUrl);
