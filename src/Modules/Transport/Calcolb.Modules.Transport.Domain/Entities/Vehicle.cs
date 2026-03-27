using Calcolb.Modules.Transport.Domain.ValueObjects;
using Calcolb.Shared.Domain;

namespace Calcolb.Modules.Transport.Domain.Entities;

public sealed class Vehicle : Entity<Guid>
{
    public Guid TransportPlanId { get; private set; }
    public PassengerCount PassengerCount { get; private set; } = null!;
    public FuelType FuelType { get; private set; } = null!;
    public FuelConsumption FuelConsumption { get; private set; } = null!;
    public RouteDestination RouteDestination { get; private set; } = null!;

    private Vehicle() { }

    public static Vehicle Create(
        Guid transportPlanId,
        PassengerCount passengerCount,
        FuelType fuelType,
        FuelConsumption fuelConsumption,
        string origin,
        string destination)
    {
        var vehicle = new Vehicle
        {
            Id = Guid.NewGuid(),
            TransportPlanId = transportPlanId,
            PassengerCount = passengerCount,
            FuelType = fuelType,
            FuelConsumption = fuelConsumption
        };

        vehicle.RouteDestination = RouteDestination.Create(vehicle.Id, origin, destination);

        return vehicle;
    }
}
