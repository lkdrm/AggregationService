using AggregationService.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace AggregationService.Sql;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    { }

    public DbSet<ProductReadModel> Products => Set<ProductReadModel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProductReadModel>().HasKey(product => product.Id);
        modelBuilder.Entity<ProductReadModel>().OwnsOne(product => product.PriceDetails);
        modelBuilder.Entity<ProductReadModel>().OwnsOne(product => product.StockDetails);
    }
}
