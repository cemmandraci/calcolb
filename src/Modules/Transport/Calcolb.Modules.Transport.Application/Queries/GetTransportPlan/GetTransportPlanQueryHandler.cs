using Calcolb.Modules.Transport.Application.Repositories;
using Mediator;

namespace Calcolb.Modules.Transport.Application.Queries.GetTransportPlan;

public sealed class GetTransportPlanQueryHandler : IQueryHandler<GetTransportPlanQuery, GetTransportPlanResult?>
{
    private readonly ITransportPlanRepository _repository;

    public GetTransportPlanQueryHandler(ITransportPlanRepository repository)
    {
        _repository = repository;
    }

    public async ValueTask<GetTransportPlanResult?> Handle(
        GetTransportPlanQuery query,
        CancellationToken cancellationToken)
    {
        var plan = await _repository.GetByIdAsync(query.TransportPlanId, cancellationToken);

        if (plan is null)
            return null;

        var vehicles = plan.Vehicles.Select(v => new VehicleDto(
            v.Id,
            v.PassengerCount.Value,
            v.FuelType.Value,
            v.FuelConsumption.Value,
            v.RouteDestination.Origin,
            v.RouteDestination.Destination,
            v.RouteDestination.Distance)).ToList();

        return new GetTransportPlanResult(
            plan.Id,
            plan.EventId,
            plan.TransportType.Value,
            plan.ParticipantCount,
            plan.TotalCost,
            plan.PublicTransportCost?.CostPerPerson,
            vehicles);
    }
}
