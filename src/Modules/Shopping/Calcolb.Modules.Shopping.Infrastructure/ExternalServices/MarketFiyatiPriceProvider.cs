using Calcolb.Modules.Shopping.Application.Services;

namespace Calcolb.Modules.Shopping.Infrastructure.ExternalServices;

public sealed class MarketFiyatiPriceProvider : IPriceProvider
{
    public Task<decimal> GetPriceAsync(string barcode, string marketType, CancellationToken cancellationToken = default)
    {
        // TODO: Faz 1 — marketfiyati.org.tr API entegrasyonu yapılacak
        throw new NotImplementedException("marketfiyati.org.tr API entegrasyonu henüz yapılmadı.");
    }
}
