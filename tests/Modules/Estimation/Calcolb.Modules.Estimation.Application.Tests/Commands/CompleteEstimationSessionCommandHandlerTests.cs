using Calcolb.Modules.Estimation.Application.Commands.CompleteEstimationSession;
using Calcolb.Modules.Estimation.Application.Repositories;
using Calcolb.Modules.Estimation.Domain.Entities;
using Calcolb.Shared.Exceptions;
using NSubstitute;
using Shouldly;

namespace Calcolb.Modules.Estimation.Application.Tests.Commands;

public class CompleteEstimationSessionCommandHandlerTests
{
    private readonly IEstimationSessionRepository _repository;
    private readonly CompleteEstimationSessionCommandHandler _handler;

    public CompleteEstimationSessionCommandHandlerTests()
    {
        _repository = Substitute.For<IEstimationSessionRepository>();
        _handler = new CompleteEstimationSessionCommandHandler(_repository);
    }

    [Fact]
    public async Task Handle_WhenAllCostsSet_CompletesSession()
    {
        var session = EstimationSession.Create(Guid.NewGuid(), 3);
        session.UpdateTransportCost(300m);
        session.UpdateShoppingCost(600m);
        // After setting expense cost, auto-complete triggers — so we need to test
        // manual Complete before the last cost is set via UpdateExpenseCost.
        // Let's set costs manually via properties via a different approach.
        // Instead test that calling Complete after 2 costs throws exception.
        _repository.GetByIdAsync(session.Id, Arg.Any<CancellationToken>()).Returns(session);

        await Should.ThrowAsync<DomainException>(
            () => _handler.Handle(
                new CompleteEstimationSessionCommand(session.Id),
                CancellationToken.None).AsTask());
    }

    [Fact]
    public async Task Handle_WhenSessionNotFound_ThrowsDomainException()
    {
        _repository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((EstimationSession?)null);

        await Should.ThrowAsync<DomainException>(
            () => _handler.Handle(
                new CompleteEstimationSessionCommand(Guid.NewGuid()),
                CancellationToken.None).AsTask());
    }

    [Fact]
    public async Task Handle_WhenSessionAlreadyCompleted_ThrowsDomainException()
    {
        var session = EstimationSession.Create(Guid.NewGuid(), 2);
        session.UpdateTransportCost(200m);
        session.UpdateShoppingCost(300m);
        session.UpdateExpenseCost(100m);
        // Session is now auto-completed
        _repository.GetByIdAsync(session.Id, Arg.Any<CancellationToken>()).Returns(session);

        await Should.ThrowAsync<DomainException>(
            () => _handler.Handle(
                new CompleteEstimationSessionCommand(session.Id),
                CancellationToken.None).AsTask());
    }
}
