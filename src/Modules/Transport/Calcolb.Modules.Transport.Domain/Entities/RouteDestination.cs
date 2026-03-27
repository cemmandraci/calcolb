using Calcolb.Shared.Domain;
using Calcolb.Shared.Exceptions;

namespace Calcolb.Modules.Transport.Domain.Entities;

public sealed class RouteDestination : Entity<Guid>
{
    public Guid VehicleId { get; private set; }
    public string Origin { get; private set; } = null!;
    public string Destination { get; private set; } = null!;
    public decimal Distance { get; private set; }

    private RouteDestination() { }

    public static RouteDestination Create(Guid vehicleId, string origin, string destination)
    {
        if (string.IsNullOrWhiteSpace(origin))
            throw new DomainException("Başlangıç noktası zorunludur.");

        if (string.IsNullOrWhiteSpace(destination))
            throw new DomainException("Varış noktası zorunludur.");

        return new RouteDestination
        {
            Id = Guid.NewGuid(),
            VehicleId = vehicleId,
            Origin = origin,
            Destination = destination,
            Distance = 0
        };
    }

    public void SetDistance(decimal distance)
    {
        if (distance < 0)
            throw new DomainException("Mesafe negatif olamaz.");

        Distance = distance;
    }
}
