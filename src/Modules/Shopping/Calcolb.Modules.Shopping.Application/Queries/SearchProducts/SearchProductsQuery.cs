using Mediator;

namespace Calcolb.Modules.Shopping.Application.Queries.SearchProducts;

public sealed record SearchProductsQuery(string SearchTerm) : IQuery<List<ProductResult>>;

public sealed record ProductResult(
    Guid ProductId,
    string Name,
    Guid CategoryId,
    string Barcode,
    string Unit,
    string MarketType,
    bool IsActive);
