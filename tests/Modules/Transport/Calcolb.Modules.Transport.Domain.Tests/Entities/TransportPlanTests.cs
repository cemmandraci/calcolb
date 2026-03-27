using Calcolb.Modules.Transport.Domain.Entities;
using Calcolb.Modules.Transport.Domain.Events;
using Calcolb.Modules.Transport.Domain.ValueObjects;
using Calcolb.Shared.Exceptions;
using Shouldly;

namespace Calcolb.Modules.Transport.Domain.Tests.Entities;

public class TransportPlanTests
{
    private readonly Guid _eventId = Guid.NewGuid();

    // --- Create Factory Method Tests ---

    [Fact]
    public void Create_WhenArabayla_ReturnsTransportPlan()
    {
        var plan = TransportPlan.Create(_eventId, TransportType.Arabayla, 5);

        plan.ShouldNotBeNull();
        plan.Id.ShouldNotBe(Guid.Empty);
        plan.EventId.ShouldBe(_eventId);
        plan.TransportType.ShouldBe(TransportType.Arabayla);
        plan.ParticipantCount.ShouldBe(5);
        plan.PublicTransportCost.ShouldBeNull();
        plan.Vehicles.ShouldBeEmpty();
    }

    [Fact]
    public void Create_WhenTopluTasimaWithCost_ReturnsTransportPlan()
    {
        var cost = PublicTransportCost.Create(15m, 5);

        var plan = TransportPlan.Create(_eventId, TransportType.TopluTasima, 5, cost);

        plan.ShouldNotBeNull();
        plan.TransportType.ShouldBe(TransportType.TopluTasima);
        plan.PublicTransportCost.ShouldNotBeNull();
        plan.PublicTransportCost.ShouldBe(cost);
    }

    [Fact]
    public void Create_WhenTopluTasimaWithoutCost_ThrowsDomainException()
    {
        Should.Throw<DomainException>(() =>
            TransportPlan.Create(_eventId, TransportType.TopluTasima, 5))
            .Message.ShouldContain("bilet ücreti zorunludur");
    }

    [Fact]
    public void Create_WhenYuruyerek_ReturnsTransportPlan()
    {
        var plan = TransportPlan.Create(_eventId, TransportType.Yuruyerek, 3);

        plan.ShouldNotBeNull();
        plan.TransportType.ShouldBe(TransportType.Yuruyerek);
        plan.PublicTransportCost.ShouldBeNull();
    }

    [Fact]
    public void Create_WhenYuruyerekWithCost_ThrowsDomainException()
    {
        var cost = PublicTransportCost.Create(10m, 3);

        Should.Throw<DomainException>(() =>
            TransportPlan.Create(_eventId, TransportType.Yuruyerek, 3, cost))
            .Message.ShouldContain("toplu taşıma ücreti girilmemelidir");
    }

    [Fact]
    public void Create_WhenArabaylaWithCost_ThrowsDomainException()
    {
        var cost = PublicTransportCost.Create(10m, 5);

        Should.Throw<DomainException>(() =>
            TransportPlan.Create(_eventId, TransportType.Arabayla, 5, cost))
            .Message.ShouldContain("toplu taşıma ücreti girilmemelidir");
    }

    [Fact]
    public void Create_WhenParticipantCountIsZero_ThrowsDomainException()
    {
        Should.Throw<DomainException>(() =>
            TransportPlan.Create(_eventId, TransportType.Arabayla, 0))
            .Message.ShouldContain("en az 1");
    }

    [Fact]
    public void Create_GeneratesUniqueIds()
    {
        var plan1 = TransportPlan.Create(_eventId, TransportType.Arabayla, 2);
        var plan2 = TransportPlan.Create(_eventId, TransportType.Arabayla, 2);

        plan1.Id.ShouldNotBe(plan2.Id);
    }

    // --- AddVehicle Tests ---

    [Fact]
    public void AddVehicle_WhenArabayla_AddsVehicle()
    {
        var plan = TransportPlan.Create(_eventId, TransportType.Arabayla, 3);
        var vehicle = Vehicle.Create(
            plan.Id,
            PassengerCount.Create(3),
            FuelType.Benzin,
            FuelConsumption.Create(),
            "Ankara",
            "İstanbul");

        plan.AddVehicle(vehicle);

        plan.Vehicles.Count.ShouldBe(1);
        plan.Vehicles[0].ShouldBe(vehicle);
    }

    [Fact]
    public void AddVehicle_WhenTopluTasima_ThrowsDomainException()
    {
        var cost = PublicTransportCost.Create(10m, 3);
        var plan = TransportPlan.Create(_eventId, TransportType.TopluTasima, 3, cost);
        var vehicle = Vehicle.Create(
            plan.Id,
            PassengerCount.Create(3),
            FuelType.Benzin,
            FuelConsumption.Create(),
            "Ankara",
            "İstanbul");

        Should.Throw<DomainException>(() => plan.AddVehicle(vehicle))
            .Message.ShouldContain("Arabayla");
    }

    [Fact]
    public void AddVehicle_WhenYuruyerek_ThrowsDomainException()
    {
        var plan = TransportPlan.Create(_eventId, TransportType.Yuruyerek, 3);
        var vehicle = Vehicle.Create(
            plan.Id,
            PassengerCount.Create(3),
            FuelType.Benzin,
            FuelConsumption.Create(),
            "Ankara",
            "İstanbul");

        Should.Throw<DomainException>(() => plan.AddVehicle(vehicle))
            .Message.ShouldContain("Arabayla");
    }

    // --- CalculateCost Tests ---

    [Fact]
    public void CalculateCost_WhenArabaylaAndPassengerCountMatches_CalculatesCorrectly()
    {
        var plan = TransportPlan.Create(_eventId, TransportType.Arabayla, 3);
        var vehicle = Vehicle.Create(
            plan.Id,
            PassengerCount.Create(3),
            FuelType.Benzin,
            FuelConsumption.Create(10m),
            "Ankara",
            "İstanbul");
        vehicle.RouteDestination.SetDistance(450m);
        plan.AddVehicle(vehicle);

        var fuelPrices = new Dictionary<Guid, decimal> { [vehicle.Id] = 40m };

        plan.CalculateCost(fuelPrices);

        // (450 / 100) * 10 * 40 = 4.5 * 10 * 40 = 1800
        plan.TotalCost.ShouldBe(1800m);
    }

    [Fact]
    public void CalculateCost_WhenArabaylaWithMultipleVehicles_SumsCosts()
    {
        var plan = TransportPlan.Create(_eventId, TransportType.Arabayla, 5);

        var vehicle1 = Vehicle.Create(
            plan.Id, PassengerCount.Create(3), FuelType.Benzin,
            FuelConsumption.Create(10m), "Ankara", "İstanbul");
        vehicle1.RouteDestination.SetDistance(450m);

        var vehicle2 = Vehicle.Create(
            plan.Id, PassengerCount.Create(2), FuelType.Dizel,
            FuelConsumption.Create(6m), "Ankara", "İstanbul");
        vehicle2.RouteDestination.SetDistance(450m);

        plan.AddVehicle(vehicle1);
        plan.AddVehicle(vehicle2);

        var fuelPrices = new Dictionary<Guid, decimal>
        {
            [vehicle1.Id] = 40m,
            [vehicle2.Id] = 38m
        };

        plan.CalculateCost(fuelPrices);

        // Vehicle1: (450/100) * 10 * 40 = 1800
        // Vehicle2: (450/100) * 6 * 38 = 1026
        plan.TotalCost.ShouldBe(1800m + 1026m);
    }

    [Fact]
    public void CalculateCost_WhenArabaylaPassengerCountMismatch_ThrowsDomainException()
    {
        var plan = TransportPlan.Create(_eventId, TransportType.Arabayla, 5);
        var vehicle = Vehicle.Create(
            plan.Id, PassengerCount.Create(3), FuelType.Benzin,
            FuelConsumption.Create(), "Ankara", "İstanbul");
        vehicle.RouteDestination.SetDistance(100m);
        plan.AddVehicle(vehicle);

        var fuelPrices = new Dictionary<Guid, decimal> { [vehicle.Id] = 40m };

        Should.Throw<DomainException>(() => plan.CalculateCost(fuelPrices))
            .Message.ShouldContain("eşit olmalıdır");
    }

    [Fact]
    public void CalculateCost_WhenArabaylaNoVehicles_ThrowsDomainException()
    {
        var plan = TransportPlan.Create(_eventId, TransportType.Arabayla, 3);

        Should.Throw<DomainException>(() => plan.CalculateCost(new Dictionary<Guid, decimal>()))
            .Message.ShouldContain("en az bir araç");
    }

    [Fact]
    public void CalculateCost_WhenTopluTasima_ReturnsTotalAmount()
    {
        var cost = PublicTransportCost.Create(15m, 4);
        var plan = TransportPlan.Create(_eventId, TransportType.TopluTasima, 4, cost);

        plan.CalculateCost(new Dictionary<Guid, decimal>());

        plan.TotalCost.ShouldBe(60m);
    }

    [Fact]
    public void CalculateCost_WhenYuruyerek_ReturnsZero()
    {
        var plan = TransportPlan.Create(_eventId, TransportType.Yuruyerek, 3);

        plan.CalculateCost(new Dictionary<Guid, decimal>());

        plan.TotalCost.ShouldBe(0m);
    }

    [Fact]
    public void CalculateCost_RaisesTransportPlanCalculatedDomainEvent()
    {
        var plan = TransportPlan.Create(_eventId, TransportType.Yuruyerek, 2);

        plan.CalculateCost(new Dictionary<Guid, decimal>());

        plan.DomainEvents.Count.ShouldBe(1);
        var domainEvent = plan.DomainEvents.First().ShouldBeOfType<TransportPlanCalculatedDomainEvent>();
        domainEvent.TransportPlanId.ShouldBe(plan.Id);
        domainEvent.RelatedEventId.ShouldBe(_eventId);
        domainEvent.TotalCost.ShouldBe(0m);
        domainEvent.OccurredOn.ShouldNotBe(default);
        domainEvent.EventId.ShouldNotBe(Guid.Empty);
    }
}
