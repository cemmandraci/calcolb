using Calcolb.Modules.Shopping.Application.Repositories;
using Mediator;

namespace Calcolb.Modules.Shopping.Application.Queries.GetShoppingCart;

public sealed class GetShoppingCartQueryHandler : IQueryHandler<GetShoppingCartQuery, GetShoppingCartResult?>
{
    private readonly IShoppingCartRepository _cartRepository;

    public GetShoppingCartQueryHandler(IShoppingCartRepository cartRepository)
    {
        _cartRepository = cartRepository;
    }

    public async ValueTask<GetShoppingCartResult?> Handle(
        GetShoppingCartQuery query,
        CancellationToken cancellationToken)
    {
        var cart = await _cartRepository.GetByIdAsync(query.ShoppingCartId, cancellationToken);

        if (cart is null)
            return null;

        var items = cart.CartItems.Select(ci => new CartItemDto(
            ci.Id,
            ci.ProductId,
            ci.Quantity,
            ci.Unit,
            ci.MarketType,
            ci.UnitPrice,
            ci.TotalPrice)).ToList();

        return new GetShoppingCartResult(cart.Id, cart.EventId, cart.TotalCost, items);
    }
}
