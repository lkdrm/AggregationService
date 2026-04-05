using AggregationService.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AggregationService.Controllers;

/// <summary>
/// API controller that exposes endpoints for retrieving aggregated product data.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of <see cref="ProductsController"/>.
    /// </summary>
    /// <param name="mediator">The MediatR mediator used to dispatch queries.</param>
    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves the fully aggregated product data for the specified ID.
    /// </summary>
    /// <param name="id">The unique identifier of the product.</param>
    /// <returns>
    /// <see cref="OkObjectResult"/> containing the aggregated product if found;
    /// otherwise, <see cref="NotFoundResult"/>.
    /// </returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct(string id)
    {
        var query = new GetAggregatedProductQuery(id);

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
