using Calcolb.Shared.Domain;

namespace Calcolb.Modules.Expense.Domain.Events;

public sealed class ExpensePlanCalculatedDomainEvent : IDomainEvent
{
    public Guid EventId { get; }
    public DateTime OccurredOn { get; }
    public Guid ExpensePlanId { get; }
    public Guid RelatedEventId { get; }
    public decimal TotalCost { get; }

    public ExpensePlanCalculatedDomainEvent(Guid expensePlanId, Guid relatedEventId, decimal totalCost)
    {
        EventId = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
        ExpensePlanId = expensePlanId;
        RelatedEventId = relatedEventId;
        TotalCost = totalCost;
    }
}
