using AggregationService.Application.Interfaces;
using AggregationService.Domain.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AggregationService.Application.Queries;

public class GetAggregatedProductHandler : IRequestHandler<GetAggregatedProductQuery, AggregatedProduct>
{
    private readonly ILogger<GetAggregatedProductHandler> _logger;
    private readonly IProductClient _productClient;
    private readonly IPricingClient _pricingClient;
    private readonly IStockClient _stockClient;

    public GetAggregatedProductHandler(ILogger<GetAggregatedProductHandler> logger, IProductClient productClient, IPricingClient pricingClient, IStockClient stockClient)
    {
        _logger = logger;
        _productClient = productClient;
        _pricingClient = pricingClient;
        _stockClient = stockClient;
    }

    async Task<AggregatedProduct> IRequestHandler<GetAggregatedProductQuery, AggregatedProduct>.Handle(GetAggregatedProductQuery request, CancellationToken cancellationToken)
    {
        var price = GetPriceDetailsAsync(request.Id);
        var product = _productClient.GetProductItemAsync(request.Id);
        var stock = _stockClient.GetStockDetailsAsync(request.Id);

        await Task.WhenAll(product, price, stock);

        var priceData = await price;
        var productData = await product;
        var stockData = await stock;

        return new AggregatedProduct(request.Id, productData.Name, productData.ImageUrl, priceData, stockData);
    }

    private async Task<PriceDetails?> GetPriceDetailsAsync(string id)
    {
        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            return await _pricingClient.GetPriceDetailsAsync(id).WaitAsync(cts.Token);
        }
        catch (Exception ex)
        {
            _logger.LogWarning($"Failed to fetch price, applying fallback. Error: {ex.Message}");
            return null;
        }
    }
}
