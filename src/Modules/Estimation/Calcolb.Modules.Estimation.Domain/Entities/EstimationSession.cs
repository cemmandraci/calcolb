using Calcolb.Modules.Estimation.Domain.Enums;
using Calcolb.Modules.Estimation.Domain.Events;
using Calcolb.Modules.Estimation.Domain.ValueObjects;
using Calcolb.Shared.Domain;
using Calcolb.Shared.Exceptions;

namespace Calcolb.Modules.Estimation.Domain.Entities;

public sealed class EstimationSession : AggregateRoot<Guid>
{
    public Guid EventId { get; private set; }
    public int ParticipantCount { get; private set; }
    public Guid? TransportPlanId { get; private set; }
    public Guid? ShoppingCartId { get; private set; }
    public Guid? ExpensePlanId { get; private set; }
    public SessionStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public EstimationSummary? Summary { get; private set; }

    public decimal? TransportCost { get; private set; }
    public decimal? ShoppingCost { get; private set; }
    public decimal? ExpenseCost { get; private set; }

    private EstimationSession() { }

    public static EstimationSession Create(Guid eventId, int participantCount)
    {
        if (participantCount < 1)
            throw new DomainException("Katılımcı sayısı en az 1 olmalıdır.");

        return new EstimationSession
        {
            Id = Guid.NewGuid(),
            EventId = eventId,
            ParticipantCount = participantCount,
            Status = SessionStatus.InProgress,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void SetTransportPlan(Guid transportPlanId)
    {
        TransportPlanId = transportPlanId;
    }

    public void SetShoppingCart(Guid shoppingCartId)
    {
        ShoppingCartId = shoppingCartId;
    }

    public void SetExpensePlan(Guid expensePlanId)
    {
        ExpensePlanId = expensePlanId;
    }

    public void UpdateTransportCost(decimal cost)
    {
        TransportCost = cost;
        TryComplete();
    }

    public void UpdateShoppingCost(decimal cost)
    {
        ShoppingCost = cost;
        TryComplete();
    }

    public void UpdateExpenseCost(decimal cost)
    {
        ExpenseCost = cost;
        TryComplete();
    }

    public void Complete()
    {
        if (Status == SessionStatus.Completed)
            throw new DomainException("Tahmin oturumu zaten tamamlandı.");

        if (!TransportCost.HasValue || !ShoppingCost.HasValue || !ExpenseCost.HasValue)
            throw new DomainException("Tüm maliyet adımları tamamlanmadan oturum tamamlanamaz.");

        Summary = EstimationSummary.Create(
            TransportCost.Value,
            ShoppingCost.Value,
            ExpenseCost.Value,
            bufferAmount: 0m,
            ParticipantCount);

        Status = SessionStatus.Completed;
        RaiseDomainEvent(new SessionCompletedDomainEvent(Id, EventId, Summary));
    }

    private void TryComplete()
    {
        if (TransportCost.HasValue && ShoppingCost.HasValue && ExpenseCost.HasValue
            && Status == SessionStatus.InProgress)
            Complete();
    }
}
