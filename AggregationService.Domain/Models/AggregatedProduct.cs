using System;
using System.Collections.Generic;
using System.Text;

namespace AggregationService.Domain.Models;

public record AggregatedProduct(string Id, string Name, string ImageUrl, PriceDetails? Price, StockDetails? Stock);
