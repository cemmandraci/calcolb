using Mediator;

namespace Calcolb.Modules.Transport.Application.Commands.CalculateTransportCost;

public sealed record CalculateTransportCostCommand(Guid TransportPlanId) : ICommand<CalculateTransportCostResult>;

public sealed record CalculateTransportCostResult(decimal TotalCost);
