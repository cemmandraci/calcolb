using Mediator;

namespace Calcolb.Modules.Shopping.Application.Queries.GetShoppingCart;

public sealed record GetShoppingCartQuery(Guid ShoppingCartId) : IQuery<GetShoppingCartResult?>;

public sealed record GetShoppingCartResult(
    Guid ShoppingCartId,
    Guid EventId,
    decimal TotalCost,
    List<CartItemDto> CartItems);

public sealed record CartItemDto(
    Guid CartItemId,
    Guid ProductId,
    decimal Quantity,
    string Unit,
    string MarketType,
    decimal UnitPrice,
    decimal TotalPrice);
