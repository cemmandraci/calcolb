using Calcolb.Shared.Domain;
using Calcolb.Shared.Exceptions;

namespace Calcolb.Modules.Transport.Domain.ValueObjects;

public class PublicTransportCost : ValueObject
{
    public decimal CostPerPerson { get; }
    public int ParticipantCount { get; }
    public decimal TotalAmount { get; }

    private PublicTransportCost(decimal costPerPerson, int participantCount)
    {
        CostPerPerson = costPerPerson;
        ParticipantCount = participantCount;
        TotalAmount = costPerPerson * participantCount;
    }

    public static PublicTransportCost Create(decimal costPerPerson, int participantCount)
    {
        if (costPerPerson <= 0)
            throw new DomainException("Kişi başı bilet ücreti sıfırdan büyük olmalıdır.");

        if (participantCount < 1)
            throw new DomainException("Katılımcı sayısı en az 1 olmalıdır.");

        return new PublicTransportCost(costPerPerson, participantCount);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return CostPerPerson;
        yield return ParticipantCount;
    }

    public override string ToString() => $"{CostPerPerson}₺/kişi × {ParticipantCount} = {TotalAmount}₺";
}
