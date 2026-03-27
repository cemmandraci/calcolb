using Calcolb.Modules.Transport.Domain.Events;
using Calcolb.Modules.Transport.Domain.ValueObjects;
using Calcolb.Shared.Domain;
using Calcolb.Shared.Exceptions;

namespace Calcolb.Modules.Transport.Domain.Entities;

public sealed class TransportPlan : AggregateRoot<Guid>
{
    public Guid EventId { get; private set; }
    public TransportType TransportType { get; private set; } = null!;
    public int ParticipantCount { get; private set; }
    public PublicTransportCost? PublicTransportCost { get; private set; }
    public decimal TotalCost { get; private set; }

    private readonly List<Vehicle> _vehicles = [];
    public IReadOnlyList<Vehicle> Vehicles => _vehicles.AsReadOnly();

    private TransportPlan() { }

    public static TransportPlan Create(
        Guid eventId,
        TransportType transportType,
        int participantCount,
        PublicTransportCost? publicTransportCost = null)
    {
        if (participantCount < 1)
            throw new DomainException("Katılımcı sayısı en az 1 olmalıdır.");

        if (transportType == TransportType.TopluTasima && publicTransportCost is null)
            throw new DomainException("Toplu taşıma seçildiğinde bilet ücreti zorunludur.");

        if (transportType == TransportType.Arabayla && publicTransportCost is not null)
            throw new DomainException("Arabayla seçildiğinde toplu taşıma ücreti girilmemelidir.");

        if (transportType == TransportType.Yuruyerek && publicTransportCost is not null)
            throw new DomainException("Yürüyerek seçildiğinde toplu taşıma ücreti girilmemelidir.");

        return new TransportPlan
        {
            Id = Guid.NewGuid(),
            EventId = eventId,
            TransportType = transportType,
            ParticipantCount = participantCount,
            PublicTransportCost = publicTransportCost
        };
    }

    public void AddVehicle(Vehicle vehicle)
    {
        if (TransportType != TransportType.Arabayla)
            throw new DomainException("Araç sadece 'Arabayla' ulaşım tipinde eklenebilir.");

        _vehicles.Add(vehicle);
    }

    public void CalculateCost(IDictionary<Guid, decimal> fuelPricesPerLiter)
    {
        if (TransportType == TransportType.Arabayla)
        {
            if (_vehicles.Count == 0)
                throw new DomainException("Arabayla ulaşım için en az bir araç eklenmelidir.");

            var totalPassengers = _vehicles.Sum(v => v.PassengerCount.Value);
            if (totalPassengers != ParticipantCount)
                throw new DomainException(
                    $"Araçlardaki toplam yolcu sayısı ({totalPassengers}) katılımcı sayısına ({ParticipantCount}) eşit olmalıdır.");

            decimal totalCost = 0;
            foreach (var vehicle in _vehicles)
            {
                if (!fuelPricesPerLiter.TryGetValue(vehicle.Id, out var fuelPrice))
                    throw new DomainException($"Araç '{vehicle.Id}' için yakıt fiyatı bulunamadı.");

                var distance = vehicle.RouteDestination.Distance;
                var consumption = vehicle.FuelConsumption.Value;
                totalCost += (distance / 100m) * consumption * fuelPrice;
            }

            TotalCost = totalCost;
        }
        else if (TransportType == TransportType.TopluTasima)
        {
            TotalCost = PublicTransportCost!.TotalAmount;
        }
        else
        {
            TotalCost = 0;
        }

        RaiseDomainEvent(new TransportPlanCalculatedDomainEvent(Id, EventId, TotalCost));
    }
}
