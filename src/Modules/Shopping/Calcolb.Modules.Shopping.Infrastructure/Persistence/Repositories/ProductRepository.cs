using Calcolb.Modules.Shopping.Application.Repositories;
using Calcolb.Modules.Shopping.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Calcolb.Modules.Shopping.Infrastructure.Persistence.Repositories;

public sealed class ProductRepository : IProductRepository
{
    private readonly ShoppingDbContext _dbContext;

    public ProductRepository(ShoppingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<List<Product>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Products
            .Where(p => p.Name.Contains(searchTerm) || p.Barcode.Contains(searchTerm))
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Product>> GetByCategoryAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Products
            .Where(p => p.CategoryId == categoryId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        await _dbContext.Products.AddAsync(product, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        _dbContext.Products.Update(product);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
