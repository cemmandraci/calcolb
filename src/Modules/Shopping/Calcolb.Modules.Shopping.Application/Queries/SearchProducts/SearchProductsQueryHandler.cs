using Calcolb.Modules.Shopping.Application.Repositories;
using Mediator;

namespace Calcolb.Modules.Shopping.Application.Queries.SearchProducts;

public sealed class SearchProductsQueryHandler : IQueryHandler<SearchProductsQuery, List<ProductResult>>
{
    private readonly IProductRepository _productRepository;

    public SearchProductsQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async ValueTask<List<ProductResult>> Handle(
        SearchProductsQuery query,
        CancellationToken cancellationToken)
    {
        var products = await _productRepository.SearchAsync(query.SearchTerm, cancellationToken);

        return products.Select(p => new ProductResult(
            p.Id,
            p.Name,
            p.CategoryId,
            p.Barcode,
            p.Unit,
            p.MarketType,
            p.IsActive)).ToList();
    }
}
