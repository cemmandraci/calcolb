using Mediator;

namespace Calcolb.Modules.Transport.Application.Queries.GetTransportPlan;

public sealed record GetTransportPlanQuery(Guid TransportPlanId) : IQuery<GetTransportPlanResult?>;

public sealed record GetTransportPlanResult(
    Guid TransportPlanId,
    Guid EventId,
    string TransportType,
    int ParticipantCount,
    decimal TotalCost,
    decimal? CostPerPerson,
    List<VehicleDto> Vehicles);

public sealed record VehicleDto(
    Guid VehicleId,
    int PassengerCount,
    string FuelType,
    decimal FuelConsumption,
    string Origin,
    string Destination,
    decimal Distance);
