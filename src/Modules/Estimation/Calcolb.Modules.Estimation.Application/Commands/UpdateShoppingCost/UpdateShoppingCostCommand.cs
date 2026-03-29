using Mediator;

namespace Calcolb.Modules.Estimation.Application.Commands.UpdateShoppingCost;

public sealed record UpdateShoppingCostCommand(
    Guid SessionId,
    Guid ShoppingCartId,
    decimal Cost) : ICommand<UpdateShoppingCostResult>;

public sealed record UpdateShoppingCostResult(bool SessionCompleted);
