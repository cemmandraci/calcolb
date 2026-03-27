using Mediator;

namespace Calcolb.Modules.Transport.Application.Commands.CreateTransportPlan;

public sealed record CreateTransportPlanCommand(
    Guid EventId,
    string TransportType,
    int ParticipantCount,
    decimal? CostPerPerson) : ICommand<CreateTransportPlanResult>;

public sealed record CreateTransportPlanResult(Guid TransportPlanId);
