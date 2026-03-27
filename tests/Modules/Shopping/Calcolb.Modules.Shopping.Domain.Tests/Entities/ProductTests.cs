using Calcolb.Modules.Shopping.Domain.Entities;
using Calcolb.Shared.Exceptions;
using Shouldly;

namespace Calcolb.Modules.Shopping.Domain.Tests.Entities;

public class ProductTests
{
    private static Product CreateActiveProduct() =>
        Product.Create("Süt", Guid.NewGuid(), "1234567890", "litre", "Migros", true);

    private static Product CreateInactiveProduct() =>
        Product.Create("Ekmek", Guid.NewGuid(), "0987654321", "adet", "Bim", false);

    [Fact]
    public void Create_WhenValidParameters_ReturnsProduct()
    {
        var categoryId = Guid.NewGuid();

        var product = Product.Create("Süt", categoryId, "1234567890", "litre", "Migros");

        product.ShouldNotBeNull();
        product.Id.ShouldNotBe(Guid.Empty);
        product.Name.ShouldBe("Süt");
        product.CategoryId.ShouldBe(categoryId);
        product.Barcode.ShouldBe("1234567890");
        product.Unit.ShouldBe("litre");
        product.MarketType.ShouldBe("Migros");
        product.IsActive.ShouldBeTrue();
    }

    [Fact]
    public void Create_WhenIsActiveFalse_CreatesInactiveProduct()
    {
        var product = Product.Create("Ekmek", Guid.NewGuid(), "111", "adet", "Bim", false);

        product.IsActive.ShouldBeFalse();
    }

    [Fact]
    public void Create_WhenNameIsEmpty_ThrowsDomainException()
    {
        Should.Throw<DomainException>(() =>
            Product.Create("", Guid.NewGuid(), "111", "adet", "Migros"));
    }

    [Fact]
    public void Create_WhenCategoryIdIsEmpty_ThrowsDomainException()
    {
        Should.Throw<DomainException>(() =>
            Product.Create("Süt", Guid.Empty, "111", "litre", "Migros"));
    }

    [Fact]
    public void Create_WhenBarcodeIsEmpty_ThrowsDomainException()
    {
        Should.Throw<DomainException>(() =>
            Product.Create("Süt", Guid.NewGuid(), "", "litre", "Migros"));
    }

    [Fact]
    public void Deactivate_SetsIsActiveFalse()
    {
        var product = CreateActiveProduct();

        product.Deactivate();

        product.IsActive.ShouldBeFalse();
    }

    [Fact]
    public void Activate_SetsIsActiveTrue()
    {
        var product = CreateInactiveProduct();

        product.Activate();

        product.IsActive.ShouldBeTrue();
    }

    [Fact]
    public void Create_GeneratesUniqueIds()
    {
        var p1 = CreateActiveProduct();
        var p2 = CreateActiveProduct();

        p1.Id.ShouldNotBe(p2.Id);
    }
}
