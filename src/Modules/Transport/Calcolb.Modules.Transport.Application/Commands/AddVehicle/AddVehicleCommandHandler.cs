using Calcolb.Modules.Transport.Application.Repositories;
using Calcolb.Modules.Transport.Domain.Entities;
using Calcolb.Modules.Transport.Domain.ValueObjects;
using Calcolb.Shared.Exceptions;
using Mediator;

namespace Calcolb.Modules.Transport.Application.Commands.AddVehicle;

public sealed class AddVehicleCommandHandler : ICommandHandler<AddVehicleCommand, AddVehicleResult>
{
    private readonly ITransportPlanRepository _repository;

    public AddVehicleCommandHandler(ITransportPlanRepository repository)
    {
        _repository = repository;
    }

    public async ValueTask<AddVehicleResult> Handle(AddVehicleCommand command, CancellationToken cancellationToken)
    {
        var plan = await _repository.GetByIdAsync(command.TransportPlanId, cancellationToken)
            ?? throw new DomainException("Ulaşım planı bulunamadı.");

        var passengerCount = PassengerCount.Create(command.PassengerCount);
        var fuelType = FuelType.FromString(command.FuelType);
        var fuelConsumption = FuelConsumption.Create(command.FuelConsumption);

        var vehicle = Vehicle.Create(
            plan.Id,
            passengerCount,
            fuelType,
            fuelConsumption,
            command.Origin,
            command.Destination);

        plan.AddVehicle(vehicle);

        await _repository.UpdateAsync(plan, cancellationToken);

        return new AddVehicleResult(vehicle.Id);
    }
}
