using Calcolb.Shared.Domain;

namespace Calcolb.Modules.Shopping.Domain.Events;

public sealed class ShoppingCartCalculatedDomainEvent : IDomainEvent
{
    public Guid EventId { get; }
    public DateTime OccurredOn { get; }
    public Guid ShoppingCartId { get; }
    public Guid RelatedEventId { get; }
    public decimal TotalCost { get; }

    public ShoppingCartCalculatedDomainEvent(Guid shoppingCartId, Guid relatedEventId, decimal totalCost)
    {
        EventId = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
        ShoppingCartId = shoppingCartId;
        RelatedEventId = relatedEventId;
        TotalCost = totalCost;
    }
}
