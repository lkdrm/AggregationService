using AggregationService.Application.Interfaces;
using AggregationService.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace AggregationService.Sql.Repositories;

/// <summary>
/// SQL-backed repository implementation for retrieving aggregated product data.
/// </summary>
public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Initializes a new instance of <see cref="ProductRepository"/> with the given database context.
    /// </summary>
    /// <param name="context">The EF Core database context used to query product data.</param>
    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves an aggregated product by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the product to retrieve.</param>
    /// <param name="cancellationToken">Token used to cancel the asynchronous operation.</param>
    /// <returns>
    /// An <see cref="AggregatedProduct"/> if a matching product is found; otherwise, <see langword="null"/>.
    /// </returns>
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
