using Calcolb.Modules.Transport.Application.Repositories;
using Calcolb.Modules.Transport.Application.Services;
using Calcolb.Modules.Transport.Domain.ValueObjects;
using Calcolb.Shared.Exceptions;
using Mediator;

namespace Calcolb.Modules.Transport.Application.Commands.CalculateTransportCost;

public sealed class CalculateTransportCostCommandHandler
    : ICommandHandler<CalculateTransportCostCommand, CalculateTransportCostResult>
{
    private readonly ITransportPlanRepository _repository;
    private readonly IFuelPriceProvider _fuelPriceProvider;
    private readonly IRouteCalculator _routeCalculator;

    public CalculateTransportCostCommandHandler(
        ITransportPlanRepository repository,
        IFuelPriceProvider fuelPriceProvider,
        IRouteCalculator routeCalculator)
    {
        _repository = repository;
        _fuelPriceProvider = fuelPriceProvider;
        _routeCalculator = routeCalculator;
    }

    public async ValueTask<CalculateTransportCostResult> Handle(
        CalculateTransportCostCommand command,
        CancellationToken cancellationToken)
    {
        var plan = await _repository.GetByIdAsync(command.TransportPlanId, cancellationToken)
            ?? throw new DomainException("Ulaşım planı bulunamadı.");

        var fuelPrices = new Dictionary<Guid, decimal>();

        if (plan.TransportType == TransportType.Arabayla)
        {
            foreach (var vehicle in plan.Vehicles)
            {
                var distance = await _routeCalculator.GetDistanceAsync(
                    vehicle.RouteDestination.Origin,
                    vehicle.RouteDestination.Destination,
                    cancellationToken);
                vehicle.RouteDestination.SetDistance(distance);

                var fuelPrice = await _fuelPriceProvider.GetPricePerLiterAsync(
                    vehicle.FuelType.Value,
                    cancellationToken);
                fuelPrices[vehicle.Id] = fuelPrice;
            }
        }

        plan.CalculateCost(fuelPrices);

        await _repository.UpdateAsync(plan, cancellationToken);

        return new CalculateTransportCostResult(plan.TotalCost);
    }
}
