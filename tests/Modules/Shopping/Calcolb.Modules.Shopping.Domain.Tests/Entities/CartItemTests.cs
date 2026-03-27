using Calcolb.Modules.Shopping.Domain.Entities;
using Calcolb.Shared.Exceptions;
using Shouldly;

namespace Calcolb.Modules.Shopping.Domain.Tests.Entities;

public class CartItemTests
{
    [Fact]
    public void Create_WhenValidParameters_ReturnsCartItem()
    {
        var cartItem = CartItem.Create(Guid.NewGuid(), Guid.NewGuid(), 2m, "kg", "Migros", 50m);

        cartItem.ShouldNotBeNull();
        cartItem.Id.ShouldNotBe(Guid.Empty);
        cartItem.Quantity.ShouldBe(2m);
        cartItem.Unit.ShouldBe("kg");
        cartItem.MarketType.ShouldBe("Migros");
        cartItem.UnitPrice.ShouldBe(50m);
    }

    [Fact]
    public void Create_TotalPrice_IsQuantityMultipliedByUnitPrice()
    {
        var cartItem = CartItem.Create(Guid.NewGuid(), Guid.NewGuid(), 3m, "kg", "Migros", 25.50m);

        cartItem.TotalPrice.ShouldBe(76.50m);
    }

    [Fact]
    public void Create_WhenDecimalQuantity_CalculatesTotalPriceCorrectly()
    {
        var cartItem = CartItem.Create(Guid.NewGuid(), Guid.NewGuid(), 0.5m, "kg", "Bim", 100m);

        cartItem.TotalPrice.ShouldBe(50m);
    }

    [Fact]
    public void Create_WhenQuantityIsZero_ThrowsDomainException()
    {
        Should.Throw<DomainException>(() =>
            CartItem.Create(Guid.NewGuid(), Guid.NewGuid(), 0m, "kg", "Migros", 50m))
            .Message.ShouldContain("sıfırdan büyük");
    }

    [Fact]
    public void Create_WhenQuantityIsNegative_ThrowsDomainException()
    {
        Should.Throw<DomainException>(() =>
            CartItem.Create(Guid.NewGuid(), Guid.NewGuid(), -1m, "kg", "Migros", 50m));
    }

    [Fact]
    public void Create_WhenUnitPriceIsZero_ThrowsDomainException()
    {
        Should.Throw<DomainException>(() =>
            CartItem.Create(Guid.NewGuid(), Guid.NewGuid(), 1m, "kg", "Migros", 0m))
            .Message.ShouldContain("sıfırdan büyük");
    }

    [Fact]
    public void Create_WhenUnitPriceIsNegative_ThrowsDomainException()
    {
        Should.Throw<DomainException>(() =>
            CartItem.Create(Guid.NewGuid(), Guid.NewGuid(), 1m, "kg", "Migros", -5m));
    }

    [Fact]
    public void Create_UnitPrice_DoesNotChangeAfterCreation()
    {
        var unitPrice = 49.99m;
        var cartItem = CartItem.Create(Guid.NewGuid(), Guid.NewGuid(), 2m, "adet", "Migros", unitPrice);

        // UnitPrice is readonly — no setter exists; this test confirms it stays fixed
        cartItem.UnitPrice.ShouldBe(unitPrice);
        cartItem.TotalPrice.ShouldBe(99.98m);
    }
}
