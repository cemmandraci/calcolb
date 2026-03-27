using Calcolb.Shared.Domain;

namespace Calcolb.Modules.Event.Domain.ValueObjects;

public class EventDate : ValueObject
{
    public DateTime Value { get; }

    private EventDate(DateTime value) => Value = value;

    public static EventDate? Create(DateTime? value)
    {
        return value.HasValue ? new EventDate(value.Value) : null;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString("yyyy-MM-dd");
}
