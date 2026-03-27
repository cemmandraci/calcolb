using Mediator;

namespace Calcolb.Modules.Transport.Application.Commands.AddVehicle;

public sealed record AddVehicleCommand(
    Guid TransportPlanId,
    int PassengerCount,
    string FuelType,
    decimal? FuelConsumption,
    string Origin,
    string Destination) : ICommand<AddVehicleResult>;

public sealed record AddVehicleResult(Guid VehicleId);
