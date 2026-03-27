using Calcolb.Modules.Transport.Application.Commands.CalculateTransportCost;
using Calcolb.Modules.Transport.Application.Repositories;
using Calcolb.Modules.Transport.Application.Services;
using Calcolb.Modules.Transport.Domain.Entities;
using Calcolb.Modules.Transport.Domain.ValueObjects;
using Calcolb.Shared.Exceptions;
using NSubstitute;
using Shouldly;

namespace Calcolb.Modules.Transport.Application.Tests.Commands;

public class CalculateTransportCostCommandHandlerTests
{
    private readonly ITransportPlanRepository _repository;
    private readonly IFuelPriceProvider _fuelPriceProvider;
    private readonly IRouteCalculator _routeCalculator;
    private readonly CalculateTransportCostCommandHandler _handler;

    public CalculateTransportCostCommandHandlerTests()
    {
        _repository = Substitute.For<ITransportPlanRepository>();
        _fuelPriceProvider = Substitute.For<IFuelPriceProvider>();
        _routeCalculator = Substitute.For<IRouteCalculator>();
        _handler = new CalculateTransportCostCommandHandler(_repository, _fuelPriceProvider, _routeCalculator);
    }

    [Fact]
    public async Task Handle_WhenArabayla_CalculatesCostCorrectly()
    {
        var plan = TransportPlan.Create(Guid.NewGuid(), TransportType.Arabayla, 3);
        var vehicle = Vehicle.Create(
            plan.Id, PassengerCount.Create(3), FuelType.Benzin,
            FuelConsumption.Create(10m), "Ankara", "İstanbul");
        plan.AddVehicle(vehicle);

        _repository.GetByIdAsync(plan.Id, Arg.Any<CancellationToken>()).Returns(plan);
        _routeCalculator.GetDistanceAsync("Ankara", "İstanbul", Arg.Any<CancellationToken>()).Returns(450m);
        _fuelPriceProvider.GetPricePerLiterAsync("Benzin", Arg.Any<CancellationToken>()).Returns(40m);

        var result = await _handler.Handle(
            new CalculateTransportCostCommand(plan.Id), CancellationToken.None);

        // (450/100) * 10 * 40 = 1800
        result.TotalCost.ShouldBe(1800m);
        await _repository.Received(1).UpdateAsync(plan, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenTopluTasima_ReturnsTotalAmount()
    {
        var cost = PublicTransportCost.Create(20m, 5);
        var plan = TransportPlan.Create(Guid.NewGuid(), TransportType.TopluTasima, 5, cost);

        _repository.GetByIdAsync(plan.Id, Arg.Any<CancellationToken>()).Returns(plan);

        var result = await _handler.Handle(
            new CalculateTransportCostCommand(plan.Id), CancellationToken.None);

        result.TotalCost.ShouldBe(100m);
    }

    [Fact]
    public async Task Handle_WhenYuruyerek_ReturnsZero()
    {
        var plan = TransportPlan.Create(Guid.NewGuid(), TransportType.Yuruyerek, 3);

        _repository.GetByIdAsync(plan.Id, Arg.Any<CancellationToken>()).Returns(plan);

        var result = await _handler.Handle(
            new CalculateTransportCostCommand(plan.Id), CancellationToken.None);

        result.TotalCost.ShouldBe(0m);
    }

    [Fact]
    public async Task Handle_WhenPlanNotFound_ThrowsDomainException()
    {
        var planId = Guid.NewGuid();
        _repository.GetByIdAsync(planId, Arg.Any<CancellationToken>()).Returns((TransportPlan?)null);

        await Should.ThrowAsync<DomainException>(
            () => _handler.Handle(
                new CalculateTransportCostCommand(planId), CancellationToken.None).AsTask());
    }

    [Fact]
    public async Task Handle_WhenArabayla_CallsRouteCalculatorAndFuelPriceProvider()
    {
        var plan = TransportPlan.Create(Guid.NewGuid(), TransportType.Arabayla, 2);
        var vehicle = Vehicle.Create(
            plan.Id, PassengerCount.Create(2), FuelType.Dizel,
            FuelConsumption.Create(7m), "İzmir", "Antalya");
        plan.AddVehicle(vehicle);

        _repository.GetByIdAsync(plan.Id, Arg.Any<CancellationToken>()).Returns(plan);
        _routeCalculator.GetDistanceAsync("İzmir", "Antalya", Arg.Any<CancellationToken>()).Returns(350m);
        _fuelPriceProvider.GetPricePerLiterAsync("Dizel", Arg.Any<CancellationToken>()).Returns(38m);

        await _handler.Handle(new CalculateTransportCostCommand(plan.Id), CancellationToken.None);

        await _routeCalculator.Received(1).GetDistanceAsync("İzmir", "Antalya", Arg.Any<CancellationToken>());
        await _fuelPriceProvider.Received(1).GetPricePerLiterAsync("Dizel", Arg.Any<CancellationToken>());
    }
}
