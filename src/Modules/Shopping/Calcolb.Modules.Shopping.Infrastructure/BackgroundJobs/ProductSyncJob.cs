using Calcolb.Modules.Shopping.Application.Repositories;
using Calcolb.Modules.Shopping.Application.Services;
using Serilog;

namespace Calcolb.Modules.Shopping.Infrastructure.BackgroundJobs;

public sealed class ProductSyncJob
{
    private readonly IProductRepository _productRepository;
    private readonly IPriceProvider _priceProvider;
    private readonly ILogger _logger;

    public ProductSyncJob(
        IProductRepository productRepository,
        IPriceProvider priceProvider,
        ILogger logger)
    {
        _productRepository = productRepository;
        _priceProvider = priceProvider;
        _logger = logger;
    }

    public Task ExecuteAsync(CancellationToken cancellationToken)
    {
        // TODO: Faz 1 — marketfiyati.org.tr API'den ürün kataloğu senkronizasyonu yapılacak
        _logger.Information("ProductSyncJob başlatıldı.");
        throw new NotImplementedException("Ürün senkronizasyon işi henüz implement edilmedi.");
    }
}
