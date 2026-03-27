using Calcolb.Modules.Shopping.Application.Queries.SearchProducts;
using Mediator;

namespace Calcolb.Modules.Shopping.Application.Queries.GetProductsByCategory;

public sealed record GetProductsByCategoryQuery(Guid CategoryId) : IQuery<List<ProductResult>>;
