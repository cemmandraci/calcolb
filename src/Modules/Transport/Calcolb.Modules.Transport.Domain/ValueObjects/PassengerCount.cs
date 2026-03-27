using Calcolb.Shared.Domain;
using Calcolb.Shared.Exceptions;

namespace Calcolb.Modules.Transport.Domain.ValueObjects;

public class PassengerCount : ValueObject
{
    public int Value { get; }

    private PassengerCount(int value) => Value = value;

    public static PassengerCount Create(int value)
    {
        if (value < 1)
            throw new DomainException("Yolcu sayısı en az 1 olmalıdır.");

        return new PassengerCount(value);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString();
}
