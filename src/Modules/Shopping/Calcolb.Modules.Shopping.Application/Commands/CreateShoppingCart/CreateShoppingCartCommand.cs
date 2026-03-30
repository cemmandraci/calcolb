using Mediator;

namespace Calcolb.Modules.Shopping.Application.Commands.CreateShoppingCart;

public sealed record CreateShoppingCartCommand(Guid EventId) : ICommand<CreateShoppingCartResult>;

public sealed record CreateShoppingCartResult(Guid ShoppingCartId);
