namespace Calcolb.Modules.Shopping.Application.Services;

public interface IPriceProvider
{
    Task<decimal> GetPriceAsync(string barcode, string marketType, CancellationToken cancellationToken = default);
}
