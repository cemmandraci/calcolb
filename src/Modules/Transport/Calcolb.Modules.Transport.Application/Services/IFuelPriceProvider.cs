namespace Calcolb.Modules.Transport.Application.Services;

public interface IFuelPriceProvider
{
    Task<decimal> GetPricePerLiterAsync(string fuelType, CancellationToken cancellationToken = default);
}
