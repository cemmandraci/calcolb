using Mediator;

namespace Calcolb.Modules.Estimation.Application.Commands.UpdateTransportCost;

public sealed record UpdateTransportCostCommand(
    Guid SessionId,
    Guid TransportPlanId,
    decimal Cost) : ICommand<UpdateTransportCostResult>;

public sealed record UpdateTransportCostResult(bool SessionCompleted);
