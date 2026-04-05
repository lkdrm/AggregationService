using AggregationService.Application.Interfaces;
using AggregationService.Domain.Models;
using AggregationService.Sql;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AggregationService.Infrastructure.Services;

public class ProductService : BackgroundService
{
    private readonly ILogger<ProductService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public ProductService(ILogger<ProductService> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Product Service is starting...");

        try
        {
            using var scope = _serviceProvider.CreateAsyncScope();

            var productClient = scope.ServiceProvider.GetRequiredService<IProductClient>();
            var priceClient = scope.ServiceProvider.GetRequiredService<IPricingClient>();
            var stockClient = scope.ServiceProvider.GetRequiredService<IStockClient>();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            for (int i = 1; i <= productClient.GetProductsCount(); i++)
            {
                _logger.LogInformation($"Sync product {i} with SQL Model...");

                var product = productClient.GetProductItemAsync(i.ToString());
                var stock = stockClient.GetStockDetailsAsync(i.ToString());
                var price = Task.Run(async () =>
                {
                    try
                    {
                        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                        return await priceClient.GetPriceDetailsAsync(i.ToString()).WaitAsync(cts.Token);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning($"Failed to fetch price for {i.ToString()}, applying fallback. Error: {ex.Message}");
                        return null;
                    }
                });

                await Task.WhenAll(product, price, stock);

                var productResult = await product;
                var priceResult = await price;
                var stockResult = await stock;

                var readModel = new ProductReadModel
                {
                    Id = productResult.Id,
                    Name = productResult.Name,
                    ImageUrl = productResult.ImageUrl,
                    PriceDetails = priceResult,
                    StockDetails = stockResult
                };

                var existingProduct = await dbContext.Products.FindAsync(i.ToString());
                if (existingProduct != null)
                {
                    dbContext.Entry(existingProduct).CurrentValues.SetValues(readModel);
                }
                else
                {
                    await dbContext.Products.AddAsync(readModel, stoppingToken);
                }
            }

            await dbContext.SaveChangesAsync(stoppingToken);
            _logger.LogInformation("SQL server updated");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while update SQL Read Model");
        }
    }
}
