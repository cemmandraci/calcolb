using Calcolb.Modules.Shopping.Domain.Events;
using Calcolb.Shared.Domain;
using Calcolb.Shared.Exceptions;

namespace Calcolb.Modules.Shopping.Domain.Entities;

public sealed class ShoppingCart : AggregateRoot<Guid>
{
    public Guid EventId { get; private set; }

    private readonly List<CartItem> _cartItems = [];
    public IReadOnlyList<CartItem> CartItems => _cartItems.AsReadOnly();

    public decimal TotalCost { get; private set; }

    private ShoppingCart() { }

    public static ShoppingCart Create(Guid eventId)
    {
        if (eventId == Guid.Empty)
            throw new DomainException("Etkinlik kimliği zorunludur.");

        return new ShoppingCart
        {
            Id = Guid.NewGuid(),
            EventId = eventId
        };
    }

    public CartItem AddItem(Product product, decimal quantity, decimal unitPrice)
    {
        if (!product.IsActive)
            throw new DomainException($"'{product.Name}' ürünü aktif değil ve sepete eklenemez.");

        var cartItem = CartItem.Create(
            Id,
            product.Id,
            quantity,
            product.Unit,
            product.MarketType,
            unitPrice);

        _cartItems.Add(cartItem);

        return cartItem;
    }

    public void RemoveItem(Guid cartItemId)
    {
        var item = _cartItems.FirstOrDefault(ci => ci.Id == cartItemId)
            ?? throw new DomainException("Sepet kalemi bulunamadı.");

        _cartItems.Remove(item);
    }

    public decimal Calculate()
    {
        TotalCost = _cartItems.Sum(ci => ci.TotalPrice);

        RaiseDomainEvent(new ShoppingCartCalculatedDomainEvent(Id, EventId, TotalCost));

        return TotalCost;
    }
}
