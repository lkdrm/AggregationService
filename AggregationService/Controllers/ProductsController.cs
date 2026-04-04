using AggregationService.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AggregationService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct(string productId)
    {
        var query = new GetAggregatedProductQuery(productId);

        var result = await _mediator.Send(query);

        if(result != null)
        {
            return Ok(result);
        }
        else
        {
            return NotFound();
        }
    }
}
