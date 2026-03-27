using Calcolb.Shared.Domain;
using Calcolb.Shared.Exceptions;

namespace Calcolb.Modules.Transport.Domain.ValueObjects;

public class FuelType : ValueObject
{
    public static readonly FuelType Benzin = new(nameof(Benzin));
    public static readonly FuelType Dizel = new(nameof(Dizel));
    public static readonly FuelType LPG = new(nameof(LPG));

    private static readonly Dictionary<string, FuelType> AllTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        [nameof(Benzin)] = Benzin,
        [nameof(Dizel)] = Dizel,
        [nameof(LPG)] = LPG
    };

    public string Value { get; }

    private FuelType(string value) => Value = value;

    public static FuelType FromString(string value)
    {
        if (!AllTypes.TryGetValue(value, out var fuelType))
            throw new DomainException($"Geçersiz yakıt tipi: '{value}'. Geçerli tipler: {string.Join(", ", AllTypes.Keys)}");

        return fuelType;
    }

    public static IReadOnlyCollection<FuelType> GetAll() => AllTypes.Values.ToList().AsReadOnly();

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
