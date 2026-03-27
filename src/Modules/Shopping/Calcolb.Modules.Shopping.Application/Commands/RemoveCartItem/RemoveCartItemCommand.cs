using Mediator;

namespace Calcolb.Modules.Shopping.Application.Commands.RemoveCartItem;

public sealed record RemoveCartItemCommand(
    Guid ShoppingCartId,
    Guid CartItemId) : ICommand<RemoveCartItemResult>;

public sealed record RemoveCartItemResult(bool Success);
