using Mediator;

namespace Calcolb.Modules.Shopping.Application.Commands.CalculateCart;

public sealed record CalculateCartCommand(Guid ShoppingCartId) : ICommand<CalculateCartResult>;

public sealed record CalculateCartResult(decimal TotalCost);
