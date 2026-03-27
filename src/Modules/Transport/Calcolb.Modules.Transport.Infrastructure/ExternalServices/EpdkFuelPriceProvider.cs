using Calcolb.Modules.Transport.Application.Services;

namespace Calcolb.Modules.Transport.Infrastructure.ExternalServices;

public sealed class EpdkFuelPriceProvider : IFuelPriceProvider
{
    public Task<decimal> GetPricePerLiterAsync(string fuelType, CancellationToken cancellationToken = default)
    {
        // TODO: Faz 1 — EPDK API entegrasyonu yapılacak
        throw new NotImplementedException("EPDK API entegrasyonu henüz yapılmadı.");
    }
}
