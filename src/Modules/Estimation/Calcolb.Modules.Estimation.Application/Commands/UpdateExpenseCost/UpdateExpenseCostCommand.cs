using Mediator;

namespace Calcolb.Modules.Estimation.Application.Commands.UpdateExpenseCost;

public sealed record UpdateExpenseCostCommand(
    Guid SessionId,
    Guid ExpensePlanId,
    decimal Cost) : ICommand<UpdateExpenseCostResult>;

public sealed record UpdateExpenseCostResult(bool SessionCompleted);
