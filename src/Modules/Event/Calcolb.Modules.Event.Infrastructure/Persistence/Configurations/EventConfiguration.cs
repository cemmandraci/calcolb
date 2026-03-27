using Calcolb.Modules.Event.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using EventEntity = Calcolb.Modules.Event.Domain.Entities.Event;

namespace Calcolb.Modules.Event.Infrastructure.Persistence.Configurations;

public sealed class EventConfiguration : IEntityTypeConfiguration<EventEntity>
{
    public void Configure(EntityTypeBuilder<EventEntity> builder)
    {
        builder.ToTable("event_events");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.ComplexProperty(e => e.EventType, eventType =>
        {
            eventType.Property(et => et.Value)
                .HasColumnName("event_type")
                .HasMaxLength(50)
                .IsRequired();
        });

        builder.ComplexProperty(e => e.ParticipantCount, participantCount =>
        {
            participantCount.Property(pc => pc.Value)
                .HasColumnName("participant_count")
                .IsRequired();
        });

        builder.OwnsOne(e => e.EventDate, eventDate =>
        {
            eventDate.Property(ed => ed.Value)
                .HasColumnName("event_date");
        });

        builder.Ignore(e => e.DomainEvents);
    }
}
