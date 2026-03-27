using Mediator;

namespace Calcolb.Modules.Shopping.Application.Commands.AddCartItem;

public sealed record AddCartItemCommand(
    Guid ShoppingCartId,
    Guid ProductId,
    decimal Quantity) : ICommand<AddCartItemResult>;

public sealed record AddCartItemResult(Guid CartItemId);
