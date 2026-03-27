using Calcolb.Modules.Shopping.Domain.Entities;

namespace Calcolb.Modules.Shopping.Application.Repositories;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Product>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<List<Product>> GetByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task AddAsync(Product product, CancellationToken cancellationToken = default);
    Task UpdateAsync(Product product, CancellationToken cancellationToken = default);
}
