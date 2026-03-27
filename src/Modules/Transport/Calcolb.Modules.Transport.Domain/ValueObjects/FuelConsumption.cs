using Calcolb.Shared.Domain;
using Calcolb.Shared.Exceptions;

namespace Calcolb.Modules.Transport.Domain.ValueObjects;

public class FuelConsumption : ValueObject
{
    public const decimal DefaultValue = 8m;

    public decimal Value { get; }

    private FuelConsumption(decimal value) => Value = value;

    public static FuelConsumption Create(decimal? value = null)
    {
        var consumption = value ?? DefaultValue;

        if (consumption <= 0)
            throw new DomainException("Yakıt tüketimi sıfırdan büyük olmalıdır.");

        return new FuelConsumption(consumption);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => $"{Value} lt/100km";
}
