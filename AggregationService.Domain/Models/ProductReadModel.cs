namespace AggregationService.Domain.Models;

/// <summary>
/// Represents the SQL read model for a product, storing aggregated data
/// from the product, pricing, and stock sources.
/// Used by Entity Framework Core as the persisted entity for the read side.
/// </summary>
public class ProductReadModel
{
    /// <summary>Gets or sets the unique identifier of the product.</summary>
    public string Id { get; set; } = null!;

    /// <summary>Gets or sets the display name of the product.</summary>
    public string Name { get; set; } = null!;

    /// <summary>Gets or sets the URL pointing to the product image.</summary>
    public string ImageUrl { get; set; } = null!;

    /// <summary>Gets or sets the pricing details. May be <see langword="null"/> if pricing is unavailable.</summary>
    public PriceDetails? PriceDetails { get; set; }

    /// <summary>Gets or sets the stock details. May be <see langword="null"/> if stock data is unavailable.</summary>
    public StockDetails? StockDetails { get; set; }
}
