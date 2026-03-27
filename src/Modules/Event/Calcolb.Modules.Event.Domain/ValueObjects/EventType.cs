using Calcolb.Shared.Domain;
using Calcolb.Shared.Exceptions;

namespace Calcolb.Modules.Event.Domain.ValueObjects;

public class EventType : ValueObject
{
    public static readonly EventType Piknik = new(nameof(Piknik));
    public static readonly EventType Kamp = new(nameof(Kamp));
    public static readonly EventType DoğumGünü = new(nameof(DoğumGünü));
    public static readonly EventType EvPartisi = new(nameof(EvPartisi));
    public static readonly EventType SporOutdoor = new(nameof(SporOutdoor));

    private static readonly Dictionary<string, EventType> AllTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        [nameof(Piknik)] = Piknik,
        [nameof(Kamp)] = Kamp,
        [nameof(DoğumGünü)] = DoğumGünü,
        [nameof(EvPartisi)] = EvPartisi,
        [nameof(SporOutdoor)] = SporOutdoor
    };

    public string Value { get; }

    private EventType(string value) => Value = value;

    public static EventType FromString(string value)
    {
        if (!AllTypes.TryGetValue(value, out var eventType))
            throw new DomainException($"Geçersiz etkinlik tipi: '{value}'. Geçerli tipler: {string.Join(", ", AllTypes.Keys)}");

        return eventType;
    }

    public static IReadOnlyCollection<EventType> GetAll() => AllTypes.Values.ToList().AsReadOnly();

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
