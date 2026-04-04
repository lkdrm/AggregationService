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
        var price = _pricingClient.GetPriceDetailsAsync(request.Id);
        var product = _productClient.GetProductItemAsync(request.Id);
        var stock = _stockClient.GetStockDetailsAsync(request.Id);

        PriceDetails priceData;

        try
        {
            await Task.WhenAll(product, price, stock);
            priceData = await price;
        }
        catch (Exception ex)
        {
            _logger.LogWarning($"Caused error during the process handling: {ex.Message}");
            priceData = null;
        }

        var productData = await product;
        var stockData = await stock;

        return new AggregatedProduct(request.Id, productData.Name, productData.ImageUrl, priceData, stockData);
    }
}
