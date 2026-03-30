using Calcolb.Modules.Shopping.Application.Queries.GetProductsByCategory;
using Calcolb.Modules.Shopping.Application.Queries.SearchProducts;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Calcolb.API.Controllers;

[ApiController]
[Route("api/products")]
[Produces("application/json")]
public sealed class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Ürün arar.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<ProductResult>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SearchProducts(
        [FromQuery] string search,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new SearchProductsQuery(search), cancellationToken);
        return Ok(result);
    }

    /// <summary>Kategoriye göre ürünleri getirir.</summary>
    [HttpGet("category/{categoryId:guid}")]
    [ProducesResponseType(typeof(List<ProductResult>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProductsByCategory(
        Guid categoryId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetProductsByCategoryQuery(categoryId), cancellationToken);
        return Ok(result);
    }
}
