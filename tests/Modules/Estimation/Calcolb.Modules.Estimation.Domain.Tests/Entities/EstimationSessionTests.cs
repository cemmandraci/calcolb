using Calcolb.Modules.Estimation.Domain.Entities;
using Calcolb.Modules.Estimation.Domain.Enums;
using Calcolb.Modules.Estimation.Domain.Events;
using Calcolb.Shared.Exceptions;
using Shouldly;

namespace Calcolb.Modules.Estimation.Domain.Tests.Entities;

public class EstimationSessionTests
{
    [Fact]
    public void Create_WithValidData_ReturnsSessionWithInProgressStatus()
    {
        var session = EstimationSession.Create(Guid.NewGuid(), 5);

        session.ShouldNotBeNull();
        session.Id.ShouldNotBe(Guid.Empty);
        session.Status.ShouldBe(SessionStatus.InProgress);
        session.ParticipantCount.ShouldBe(5);
        session.Summary.ShouldBeNull();
    }

    [Fact]
    public void Create_WithZeroParticipantCount_ThrowsDomainException()
    {
        Should.Throw<DomainException>(() =>
            EstimationSession.Create(Guid.NewGuid(), 0));
    }

    [Fact]
    public void Create_WithNegativeParticipantCount_ThrowsDomainException()
    {
        Should.Throw<DomainException>(() =>
            EstimationSession.Create(Guid.NewGuid(), -3));
    }

    [Fact]
    public void UpdateTransportCost_SetsTransportCost()
    {
        var session = EstimationSession.Create(Guid.NewGuid(), 3);

        session.UpdateTransportCost(500m);

        session.TransportCost.ShouldBe(500m);
    }

    [Fact]
    public void UpdateShoppingCost_SetsShoppingCost()
    {
        var session = EstimationSession.Create(Guid.NewGuid(), 3);

        session.UpdateShoppingCost(1000m);

        session.ShoppingCost.ShouldBe(1000m);
    }

    [Fact]
    public void UpdateExpenseCost_SetsExpenseCost()
    {
        var session = EstimationSession.Create(Guid.NewGuid(), 3);

        session.UpdateExpenseCost(250m);

        session.ExpenseCost.ShouldBe(250m);
    }

    [Fact]
    public void WhenAllCostsSet_AutoCompletesSession()
    {
        var session = EstimationSession.Create(Guid.NewGuid(), 4);

        session.UpdateTransportCost(400m);
        session.UpdateShoppingCost(800m);
        session.UpdateExpenseCost(200m);

        session.Status.ShouldBe(SessionStatus.Completed);
    }

    [Fact]
    public void WhenAllCostsSet_SummaryIsCalculated()
    {
        var session = EstimationSession.Create(Guid.NewGuid(), 4);

        session.UpdateTransportCost(400m);
        session.UpdateShoppingCost(800m);
        session.UpdateExpenseCost(200m);

        session.Summary.ShouldNotBeNull();
        session.Summary!.GrandTotal.ShouldBe(1400m);
        session.Summary.CostPerPerson.ShouldBe(350m);
    }

    [Fact]
    public void WhenAllCostsSet_RaisesSessionCompletedDomainEvent()
    {
        var session = EstimationSession.Create(Guid.NewGuid(), 2);

        session.UpdateTransportCost(200m);
        session.UpdateShoppingCost(300m);
        session.UpdateExpenseCost(100m);

        session.DomainEvents.ShouldContain(e => e is SessionCompletedDomainEvent);
    }

    [Fact]
    public void SessionNotCompleted_WhenOnlyTwoCostsSet()
    {
        var session = EstimationSession.Create(Guid.NewGuid(), 2);

        session.UpdateTransportCost(200m);
        session.UpdateShoppingCost(300m);

        session.Status.ShouldBe(SessionStatus.InProgress);
        session.Summary.ShouldBeNull();
    }

    [Fact]
    public void Complete_WhenNotAllCostsSet_ThrowsDomainException()
    {
        var session = EstimationSession.Create(Guid.NewGuid(), 2);
        session.UpdateTransportCost(200m);

        Should.Throw<DomainException>(() => session.Complete());
    }

    [Fact]
    public void Complete_WhenAlreadyCompleted_ThrowsDomainException()
    {
        var session = EstimationSession.Create(Guid.NewGuid(), 2);
        session.UpdateTransportCost(200m);
        session.UpdateShoppingCost(300m);
        session.UpdateExpenseCost(100m);

        Should.Throw<DomainException>(() => session.Complete());
    }

    [Fact]
    public void SetTransportPlan_SetsTransportPlanId()
    {
        var session = EstimationSession.Create(Guid.NewGuid(), 2);
        var planId = Guid.NewGuid();

        session.SetTransportPlan(planId);

        session.TransportPlanId.ShouldBe(planId);
    }

    [Fact]
    public void SetShoppingCart_SetsShoppingCartId()
    {
        var session = EstimationSession.Create(Guid.NewGuid(), 2);
        var cartId = Guid.NewGuid();

        session.SetShoppingCart(cartId);

        session.ShoppingCartId.ShouldBe(cartId);
    }

    [Fact]
    public void SetExpensePlan_SetsExpensePlanId()
    {
        var session = EstimationSession.Create(Guid.NewGuid(), 2);
        var planId = Guid.NewGuid();

        session.SetExpensePlan(planId);

        session.ExpensePlanId.ShouldBe(planId);
    }
}
