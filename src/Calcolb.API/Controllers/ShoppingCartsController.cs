using Calcolb.Modules.Shopping.Application.Commands.AddCartItem;
using Calcolb.Modules.Shopping.Application.Commands.CalculateCart;
using Calcolb.Modules.Shopping.Application.Commands.CreateShoppingCart;
using Calcolb.Modules.Shopping.Application.Commands.RemoveCartItem;
using Calcolb.Modules.Shopping.Application.Queries.GetShoppingCart;
using Calcolb.Shared.Exceptions;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Calcolb.API.Controllers;

[ApiController]
[Route("api/shopping-carts")]
[Produces("application/json")]
public sealed class ShoppingCartsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ShoppingCartsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Yeni bir alışveriş sepeti oluşturur.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(CreateShoppingCartResult), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateShoppingCart(
        [FromBody] CreateShoppingCartRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateShoppingCartCommand(request.EventId), cancellationToken);
        return CreatedAtAction(nameof(GetShoppingCart), new { id = result.ShoppingCartId }, result);
    }

    /// <summary>Sepete ürün ekler.</summary>
    [HttpPost("{id:guid}/items")]
    [ProducesResponseType(typeof(AddCartItemResult), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> AddCartItem(
        Guid id,
        [FromBody] AddCartItemRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AddCartItemCommand(id, request.ProductId, request.Quantity);
        var result = await _mediator.Send(command, cancellationToken);
        return Created(string.Empty, result);
    }

    /// <summary>Sepetten ürün kaldırır.</summary>
    [HttpDelete("{id:guid}/items/{itemId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RemoveCartItem(
        Guid id,
        Guid itemId,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(new RemoveCartItemCommand(id, itemId), cancellationToken);
        return NoContent();
    }

    /// <summary>Sepet toplam tutarını hesaplar.</summary>
    [HttpPost("{id:guid}/calculate")]
    [ProducesResponseType(typeof(CalculateCartResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Calculate(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CalculateCartCommand(id), cancellationToken);
        return Ok(result);
    }

    /// <summary>Sepet bilgisini getirir.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(GetShoppingCartResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetShoppingCart(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetShoppingCartQuery(id), cancellationToken);
        if (result is null) throw new NotFoundException("Sepet bulunamadı.");
        return Ok(result);
    }
}

public sealed record CreateShoppingCartRequest(Guid EventId);
public sealed record AddCartItemRequest(Guid ProductId, decimal Quantity);
