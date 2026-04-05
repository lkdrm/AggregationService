using AggregationService.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace AggregationService.Sql;

/// <summary>
/// Represents the Entity Framework Core database context for the Aggregation Service.
/// Provides access to the underlying database and manages entity configurations.
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of <see cref="AppDbContext"/> with the specified options.
    /// </summary>
    /// <param name="options">The options to configure the database context.</param>
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    { }

    /// <summary>
    /// Gets the <see cref="DbSet{TEntity}"/> for querying and saving <see cref="ProductReadModel"/> entities.
    /// </summary>
    public DbSet<ProductReadModel> Products => Set<ProductReadModel>();

    /// <summary>
    /// Configures the entity models and their relationships using the Fluent API.
    /// </summary>
    /// <param name="modelBuilder">The builder used to construct the model for the database context.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProductReadModel>().HasKey(product => product.Id);
        modelBuilder.Entity<ProductReadModel>().OwnsOne(product => product.PriceDetails);
        modelBuilder.Entity<ProductReadModel>().OwnsOne(product => product.StockDetails);
    }
}
