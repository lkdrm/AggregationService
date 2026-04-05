using AggregationService.Application.Interfaces;
using AggregationService.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace AggregationService.Sql.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<AggregatedProduct?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        var entity = await _context.Products.FirstOrDefaultAsync(product => product.Id == id, cancellationToken: cancellationToken);

        if (entity == null)
        {
            return null;
        }

        var price = entity.PriceDetails;
        var stock = new StockDetails(entity.StockDetails.Quantity, entity.StockDetails.IsAvailable);

        return new AggregatedProduct(entity.Id, entity.Name, entity.ImageUrl, price, stock);
    }
}
