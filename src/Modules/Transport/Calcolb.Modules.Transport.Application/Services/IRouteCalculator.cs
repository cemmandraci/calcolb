namespace Calcolb.Modules.Transport.Application.Services;

public interface IRouteCalculator
{
    Task<decimal> GetDistanceAsync(string origin, string destination, CancellationToken cancellationToken = default);
}
