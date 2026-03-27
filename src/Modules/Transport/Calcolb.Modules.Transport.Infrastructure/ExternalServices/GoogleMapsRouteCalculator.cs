using Calcolb.Modules.Transport.Application.Services;

namespace Calcolb.Modules.Transport.Infrastructure.ExternalServices;

public sealed class GoogleMapsRouteCalculator : IRouteCalculator
{
    public Task<decimal> GetDistanceAsync(string origin, string destination, CancellationToken cancellationToken = default)
    {
        // TODO: Faz 1 — Google Maps Directions API entegrasyonu yapılacak
        throw new NotImplementedException("Google Maps API entegrasyonu henüz yapılmadı.");
    }
}
