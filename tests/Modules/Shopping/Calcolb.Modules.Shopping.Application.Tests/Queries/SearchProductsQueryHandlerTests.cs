using Calcolb.Modules.Shopping.Application.Queries.SearchProducts;
using Calcolb.Modules.Shopping.Application.Repositories;
using Calcolb.Modules.Shopping.Domain.Entities;
using NSubstitute;
using Shouldly;

namespace Calcolb.Modules.Shopping.Application.Tests.Queries;

public class SearchProductsQueryHandlerTests
{
    private readonly IProductRepository _productRepository;
    private readonly SearchProductsQueryHandler _handler;

    public SearchProductsQueryHandlerTests()
    {
        _productRepository = Substitute.For<IProductRepository>();
        _handler = new SearchProductsQueryHandler(_productRepository);
    }

    [Fact]
    public async Task Handle_WhenProductsFound_ReturnsProductResults()
    {
        var products = new List<Product>
        {
            Product.Create("Tam Yağlı Süt", Guid.NewGuid(), "111", "litre", "Migros", true),
            Product.Create("Yarım Yağlı Süt", Guid.NewGuid(), "222", "litre", "Bim", true)
        };
        _productRepository.SearchAsync("Süt", Arg.Any<CancellationToken>()).Returns(products);

        var result = await _handler.Handle(new SearchProductsQuery("Süt"), CancellationToken.None);

        result.Count.ShouldBe(2);
        result[0].Name.ShouldBe("Tam Yağlı Süt");
        result[1].Name.ShouldBe("Yarım Yağlı Süt");
    }

    [Fact]
    public async Task Handle_WhenNoProductsFound_ReturnsEmptyList()
    {
        _productRepository.SearchAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(new List<Product>());

        var result = await _handler.Handle(new SearchProductsQuery("xyz"), CancellationToken.None);

        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task Handle_MapsProductFieldsCorrectly()
    {
        var categoryId = Guid.NewGuid();
        var product = Product.Create("Süt", categoryId, "12345", "litre", "Migros", true);
        _productRepository.SearchAsync("Süt", Arg.Any<CancellationToken>()).Returns([product]);

        var result = await _handler.Handle(new SearchProductsQuery("Süt"), CancellationToken.None);

        var dto = result.Single();
        dto.ProductId.ShouldBe(product.Id);
        dto.Name.ShouldBe("Süt");
        dto.CategoryId.ShouldBe(categoryId);
        dto.Barcode.ShouldBe("12345");
        dto.Unit.ShouldBe("litre");
        dto.MarketType.ShouldBe("Migros");
        dto.IsActive.ShouldBeTrue();
    }

    [Fact]
    public async Task Handle_IncludesInactiveProducts()
    {
        var products = new List<Product>
        {
            Product.Create("Aktif Ürün", Guid.NewGuid(), "111", "adet", "Bim", true),
            Product.Create("Pasif Ürün", Guid.NewGuid(), "222", "adet", "Bim", false)
        };
        _productRepository.SearchAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(products);

        var result = await _handler.Handle(new SearchProductsQuery("Ürün"), CancellationToken.None);

        result.Count.ShouldBe(2);
        result.Any(p => !p.IsActive).ShouldBeTrue();
    }
}
