using Calcolb.Modules.Estimation.Application.Commands.UpdateTransportCost;
using Calcolb.Modules.Estimation.Application.Commands.UpdateShoppingCost;
using Calcolb.Modules.Estimation.Application.Commands.UpdateExpenseCost;
using Calcolb.Modules.Estimation.Application.Repositories;
using Calcolb.Modules.Estimation.Domain.Entities;
using Calcolb.Modules.Estimation.Domain.Enums;
using Calcolb.Shared.Exceptions;
using NSubstitute;
using Shouldly;

namespace Calcolb.Modules.Estimation.Application.Tests.Commands;

public class UpdateCostCommandHandlerTests
{
    private readonly IEstimationSessionRepository _repository;

    public UpdateCostCommandHandlerTests()
    {
        _repository = Substitute.For<IEstimationSessionRepository>();
    }

    [Fact]
    public async Task UpdateTransportCost_WhenSessionExists_UpdatesCost()
    {
        var session = EstimationSession.Create(Guid.NewGuid(), 3);
        _repository.GetByIdAsync(session.Id, Arg.Any<CancellationToken>()).Returns(session);
        var handler = new UpdateTransportCostCommandHandler(_repository);

        var result = await handler.Handle(
            new UpdateTransportCostCommand(session.Id, Guid.NewGuid(), 500m),
            CancellationToken.None);

        result.ShouldNotBeNull();
        session.TransportCost.ShouldBe(500m);
    }

    [Fact]
    public async Task UpdateShoppingCost_WhenSessionExists_UpdatesCost()
    {
        var session = EstimationSession.Create(Guid.NewGuid(), 3);
        _repository.GetByIdAsync(session.Id, Arg.Any<CancellationToken>()).Returns(session);
        var handler = new UpdateShoppingCostCommandHandler(_repository);

        await handler.Handle(
            new UpdateShoppingCostCommand(session.Id, Guid.NewGuid(), 1000m),
            CancellationToken.None);

        session.ShoppingCost.ShouldBe(1000m);
    }

    [Fact]
    public async Task UpdateExpenseCost_WhenSessionExists_UpdatesCost()
    {
        var session = EstimationSession.Create(Guid.NewGuid(), 3);
        _repository.GetByIdAsync(session.Id, Arg.Any<CancellationToken>()).Returns(session);
        var handler = new UpdateExpenseCostCommandHandler(_repository);

        await handler.Handle(
            new UpdateExpenseCostCommand(session.Id, Guid.NewGuid(), 250m),
            CancellationToken.None);

        session.ExpenseCost.ShouldBe(250m);
    }

    [Fact]
    public async Task UpdateTransportCost_WhenAllCostsAlreadySet_ReturnsSessionCompleted()
    {
        var session = EstimationSession.Create(Guid.NewGuid(), 2);
        session.UpdateShoppingCost(800m);
        session.UpdateExpenseCost(200m);
        _repository.GetByIdAsync(session.Id, Arg.Any<CancellationToken>()).Returns(session);
        var handler = new UpdateTransportCostCommandHandler(_repository);

        var result = await handler.Handle(
            new UpdateTransportCostCommand(session.Id, Guid.NewGuid(), 400m),
            CancellationToken.None);

        result.SessionCompleted.ShouldBeTrue();
        session.Status.ShouldBe(SessionStatus.Completed);
    }

    [Fact]
    public async Task UpdateTransportCost_WhenSessionNotFound_ThrowsDomainException()
    {
        _repository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((EstimationSession?)null);
        var handler = new UpdateTransportCostCommandHandler(_repository);

        await Should.ThrowAsync<DomainException>(
            () => handler.Handle(
                new UpdateTransportCostCommand(Guid.NewGuid(), Guid.NewGuid(), 500m),
                CancellationToken.None).AsTask());
    }

    [Fact]
    public async Task UpdateTransportCost_CallsRepositoryUpdateAsync()
    {
        var session = EstimationSession.Create(Guid.NewGuid(), 3);
        _repository.GetByIdAsync(session.Id, Arg.Any<CancellationToken>()).Returns(session);
        var handler = new UpdateTransportCostCommandHandler(_repository);

        await handler.Handle(
            new UpdateTransportCostCommand(session.Id, Guid.NewGuid(), 500m),
            CancellationToken.None);

        await _repository.Received(1).UpdateAsync(session, Arg.Any<CancellationToken>());
    }
}
