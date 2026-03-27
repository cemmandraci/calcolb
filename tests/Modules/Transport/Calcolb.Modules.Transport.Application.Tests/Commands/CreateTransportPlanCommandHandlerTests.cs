using Calcolb.Modules.Transport.Application.Commands.CreateTransportPlan;
using Calcolb.Modules.Transport.Application.Repositories;
using Calcolb.Modules.Transport.Domain.Entities;
using NSubstitute;
using Shouldly;

namespace Calcolb.Modules.Transport.Application.Tests.Commands;

public class CreateTransportPlanCommandHandlerTests
{
    private readonly ITransportPlanRepository _repository;
    private readonly CreateTransportPlanCommandHandler _handler;

    public CreateTransportPlanCommandHandlerTests()
    {
        _repository = Substitute.For<ITransportPlanRepository>();
        _handler = new CreateTransportPlanCommandHandler(_repository);
    }

    [Fact]
    public async Task Handle_WhenArabayla_ReturnsResultWithTransportPlanId()
    {
        var command = new CreateTransportPlanCommand(Guid.NewGuid(), "Arabayla", 5, null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.ShouldNotBeNull();
        result.TransportPlanId.ShouldNotBe(Guid.Empty);
    }

    [Fact]
    public async Task Handle_WhenArabayla_CallsRepositoryAddAsync()
    {
        var eventId = Guid.NewGuid();
        var command = new CreateTransportPlanCommand(eventId, "Arabayla", 3, null);

        await _handler.Handle(command, CancellationToken.None);

        await _repository.Received(1).AddAsync(
            Arg.Is<TransportPlan>(tp =>
                tp.EventId == eventId &&
                tp.TransportType.Value == "Arabayla" &&
                tp.ParticipantCount == 3 &&
                tp.PublicTransportCost == null),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenTopluTasimaWithCost_CreatesWithPublicTransportCost()
    {
        var command = new CreateTransportPlanCommand(Guid.NewGuid(), "TopluTasima", 4, 15m);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.ShouldNotBeNull();
        await _repository.Received(1).AddAsync(
            Arg.Is<TransportPlan>(tp =>
                tp.TransportType.Value == "TopluTasima" &&
                tp.PublicTransportCost != null &&
                tp.PublicTransportCost.CostPerPerson == 15m &&
                tp.PublicTransportCost.TotalAmount == 60m),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenYuruyerek_CreatesWithNullPublicTransportCost()
    {
        var command = new CreateTransportPlanCommand(Guid.NewGuid(), "Yuruyerek", 2, null);

        await _handler.Handle(command, CancellationToken.None);

        await _repository.Received(1).AddAsync(
            Arg.Is<TransportPlan>(tp =>
                tp.TransportType.Value == "Yuruyerek" &&
                tp.PublicTransportCost == null),
            Arg.Any<CancellationToken>());
    }
}
