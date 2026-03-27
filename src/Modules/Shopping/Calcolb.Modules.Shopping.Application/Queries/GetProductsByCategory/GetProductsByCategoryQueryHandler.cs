using Calcolb.Modules.Shopping.Application.Queries.SearchProducts;
using Calcolb.Modules.Shopping.Application.Repositories;
using Mediator;

namespace Calcolb.Modules.Shopping.Application.Queries.GetProductsByCategory;

public sealed class GetProductsByCategoryQueryHandler
    : IQueryHandler<GetProductsByCategoryQuery, List<ProductResult>>
{
    private readonly IProductRepository _productRepository;

    public GetProductsByCategoryQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async ValueTask<List<ProductResult>> Handle(
        GetProductsByCategoryQuery query,
        CancellationToken cancellationToken)
    {
        var products = await _productRepository.GetByCategoryAsync(query.CategoryId, cancellationToken);

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
