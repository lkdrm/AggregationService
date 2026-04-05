namespace AggregationService.Domain.Models;

public class ProductReadModel
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string ImageUrl { get; set; } = null!;
    public PriceDetails? PriceDetails { get; set; }
    public StockDetails? StockDetails { get; set; }
}
