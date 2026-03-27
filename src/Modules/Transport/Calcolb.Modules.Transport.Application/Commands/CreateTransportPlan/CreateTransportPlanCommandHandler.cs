using Calcolb.Modules.Transport.Application.Repositories;
using Calcolb.Modules.Transport.Domain.Entities;
using Calcolb.Modules.Transport.Domain.ValueObjects;
using Mediator;

namespace Calcolb.Modules.Transport.Application.Commands.CreateTransportPlan;

public sealed class CreateTransportPlanCommandHandler
    : ICommandHandler<CreateTransportPlanCommand, CreateTransportPlanResult>
{
    private readonly ITransportPlanRepository _repository;

    public CreateTransportPlanCommandHandler(ITransportPlanRepository repository)
    {
        _repository = repository;
    }

    public async ValueTask<CreateTransportPlanResult> Handle(
        CreateTransportPlanCommand command,
        CancellationToken cancellationToken)
    {
        var transportType = TransportType.FromString(command.TransportType);

        PublicTransportCost? publicTransportCost = null;
        if (command.CostPerPerson.HasValue)
            publicTransportCost = PublicTransportCost.Create(command.CostPerPerson.Value, command.ParticipantCount);

        var plan = TransportPlan.Create(
            command.EventId,
            transportType,
            command.ParticipantCount,
            publicTransportCost);

        await _repository.AddAsync(plan, cancellationToken);

        return new CreateTransportPlanResult(plan.Id);
    }
}
