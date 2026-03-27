using Calcolb.Shared.Domain;
using Calcolb.Shared.Exceptions;

namespace Calcolb.Modules.Shopping.Domain.Entities;

public sealed class Product : AggregateRoot<Guid>
{
    public string Name { get; private set; } = null!;
    public Guid CategoryId { get; private set; }
    public string Barcode { get; private set; } = null!;
    public string Unit { get; private set; } = null!;
    public string MarketType { get; private set; } = null!;
    public bool IsActive { get; private set; }

    private Product() { }

    public static Product Create(
        string name,
        Guid categoryId,
        string barcode,
        string unit,
        string marketType,
        bool isActive = true)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Ürün adı zorunludur.");

        if (categoryId == Guid.Empty)
            throw new DomainException("Kategori zorunludur.");

        if (string.IsNullOrWhiteSpace(barcode))
            throw new DomainException("Barkod zorunludur.");

        if (string.IsNullOrWhiteSpace(unit))
            throw new DomainException("Birim zorunludur.");

        if (string.IsNullOrWhiteSpace(marketType))
            throw new DomainException("Market tipi zorunludur.");

        return new Product
        {
            Id = Guid.NewGuid(),
            Name = name,
            CategoryId = categoryId,
            Barcode = barcode,
            Unit = unit,
            MarketType = marketType,
            IsActive = isActive
        };
    }

    public void Deactivate() => IsActive = false;

    public void Activate() => IsActive = true;
}
