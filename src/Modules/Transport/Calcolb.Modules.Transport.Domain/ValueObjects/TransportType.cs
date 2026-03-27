using Calcolb.Shared.Domain;
using Calcolb.Shared.Exceptions;

namespace Calcolb.Modules.Transport.Domain.ValueObjects;

public class TransportType : ValueObject
{
    public static readonly TransportType Arabayla = new(nameof(Arabayla));
    public static readonly TransportType TopluTasima = new(nameof(TopluTasima));
    public static readonly TransportType Yuruyerek = new(nameof(Yuruyerek));

    private static readonly Dictionary<string, TransportType> AllTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        [nameof(Arabayla)] = Arabayla,
        [nameof(TopluTasima)] = TopluTasima,
        [nameof(Yuruyerek)] = Yuruyerek
    };

    public string Value { get; }

    private TransportType(string value) => Value = value;

    public static TransportType FromString(string value)
    {
        if (!AllTypes.TryGetValue(value, out var transportType))
            throw new DomainException($"Geçersiz ulaşım tipi: '{value}'. Geçerli tipler: {string.Join(", ", AllTypes.Keys)}");

        return transportType;
    }

    public static IReadOnlyCollection<TransportType> GetAll() => AllTypes.Values.ToList().AsReadOnly();

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
