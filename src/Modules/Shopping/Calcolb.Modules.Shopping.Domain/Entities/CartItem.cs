using Calcolb.Shared.Domain;
using Calcolb.Shared.Exceptions;

namespace Calcolb.Modules.Shopping.Domain.Entities;

public sealed class CartItem : Entity<Guid>
{
    public Guid ShoppingCartId { get; private set; }
    public Guid ProductId { get; private set; }
    public decimal Quantity { get; private set; }
    public string Unit { get; private set; } = null!;
    public string MarketType { get; private set; } = null!;
    public decimal UnitPrice { get; private set; }
    public decimal TotalPrice { get; private set; }

    private CartItem() { }

    public static CartItem Create(
        Guid shoppingCartId,
        Guid productId,
        decimal quantity,
        string unit,
        string marketType,
        decimal unitPrice)
    {
        if (quantity <= 0)
            throw new DomainException("Miktar sıfırdan büyük olmalıdır.");

        if (unitPrice <= 0)
            throw new DomainException("Birim fiyat sıfırdan büyük olmalıdır.");

        if (string.IsNullOrWhiteSpace(unit))
            throw new DomainException("Birim zorunludur.");

        if (string.IsNullOrWhiteSpace(marketType))
            throw new DomainException("Market tipi zorunludur.");

        return new CartItem
        {
            Id = Guid.NewGuid(),
            ShoppingCartId = shoppingCartId,
            ProductId = productId,
            Quantity = quantity,
            Unit = unit,
            MarketType = marketType,
            UnitPrice = unitPrice,
            TotalPrice = quantity * unitPrice
        };
    }
}
