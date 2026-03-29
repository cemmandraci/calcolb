using Calcolb.Modules.Estimation.Application.Queries.GetEstimationSummary;
using Calcolb.Modules.Estimation.Application.Repositories;
using Calcolb.Modules.Estimation.Domain.Entities;
using Calcolb.Shared.Exceptions;
using NSubstitute;
using Shouldly;

namespace Calcolb.Modules.Estimation.Application.Tests.Queries;

public class GetEstimationSummaryQueryHandlerTests
{
    private readonly IEstimationSessionRepository _repository;
    private readonly GetEstimationSummaryQueryHandler _handler;

    public GetEstimationSummaryQueryHandlerTests()
    {
        _repository = Substitute.For<IEstimationSessionRepository>();
        _handler = new GetEstimationSummaryQueryHandler(_repository);
    }

    [Fact]
    public async Task Handle_WhenSessionCompleted_ReturnsSummary()
    {
        var session = EstimationSession.Create(Guid.NewGuid(), 4);
        session.UpdateTransportCost(400m);
        session.UpdateShoppingCost(800m);
        session.UpdateExpenseCost(200m);
        _repository.GetByIdAsync(session.Id, Arg.Any<CancellationToken>()).Returns(session);

        var result = await _handler.Handle(
            new GetEstimationSummaryQuery(session.Id), CancellationToken.None);

        result.ShouldNotBeNull();
        result.TotalTransportCost.ShouldBe(400m);
        result.TotalShoppingCost.ShouldBe(800m);
        result.TotalExpenseCost.ShouldBe(200m);
        result.GrandTotal.ShouldBe(1400m);
        result.CostPerPerson.ShouldBe(350m);
    }

    [Fact]
    public async Task Handle_WhenSessionNotCompleted_ThrowsDomainException()
    {
        var session = EstimationSession.Create(Guid.NewGuid(), 3);
        session.UpdateTransportCost(300m);
        _repository.GetByIdAsync(session.Id, Arg.Any<CancellationToken>()).Returns(session);

        await Should.ThrowAsync<DomainException>(
            () => _handler.Handle(
                new GetEstimationSummaryQuery(session.Id),
                CancellationToken.None).AsTask());
    }

    [Fact]
    public async Task Handle_WhenSessionNotFound_ThrowsDomainException()
    {
        _repository.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((EstimationSession?)null);

        await Should.ThrowAsync<DomainException>(
            () => _handler.Handle(
                new GetEstimationSummaryQuery(Guid.NewGuid()),
                CancellationToken.None).AsTask());
    }

    [Fact]
    public async Task Handle_WithBufferInExpenseCost_ReflectsInSummary()
    {
        // 1000m expense cost already includes buffer (baked in from ExpensePlan)
        var session = EstimationSession.Create(Guid.NewGuid(), 2);
        session.UpdateTransportCost(200m);
        session.UpdateShoppingCost(300m);
        session.UpdateExpenseCost(1000m);
        _repository.GetByIdAsync(session.Id, Arg.Any<CancellationToken>()).Returns(session);

        var result = await _handler.Handle(
            new GetEstimationSummaryQuery(session.Id), CancellationToken.None);

        result.GrandTotal.ShouldBe(1500m);
        result.CostPerPerson.ShouldBe(750m);
        result.BufferAmount.ShouldBe(0m);
    }
}
