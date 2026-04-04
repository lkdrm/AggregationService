using System;
using System.Collections.Generic;
using System.Text;

namespace AggregationService.Domain.Models;

public record StockDetails(int Quantity, bool IsAvailable);
