using Calcolb.Shared.Domain;
using Calcolb.Shared.Exceptions;

namespace Calcolb.Modules.Event.Domain.ValueObjects;

public class ParticipantCount : ValueObject
{
    public int Value { get; }

    private ParticipantCount(int value) => Value = value;

    public static ParticipantCount Create(int value)
    {
        if (value < 1)
            throw new DomainException("Katılımcı sayısı en az 1 olmalıdır.");

        return new ParticipantCount(value);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString();
}
