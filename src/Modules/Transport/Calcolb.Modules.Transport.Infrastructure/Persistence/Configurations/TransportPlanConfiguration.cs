using Calcolb.Modules.Transport.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Calcolb.Modules.Transport.Infrastructure.Persistence.Configurations;

public sealed class TransportPlanConfiguration : IEntityTypeConfiguration<TransportPlan>
{
    public void Configure(EntityTypeBuilder<TransportPlan> builder)
    {
        builder.ToTable("transport_plans");

        builder.HasKey(tp => tp.Id);

        builder.Property(tp => tp.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(tp => tp.EventId)
            .HasColumnName("event_id")
            .IsRequired();

        builder.ComplexProperty(tp => tp.TransportType, transportType =>
        {
            transportType.Property(tt => tt.Value)
                .HasColumnName("transport_type")
                .HasMaxLength(50)
                .IsRequired();
        });

        builder.Property(tp => tp.ParticipantCount)
            .HasColumnName("participant_count")
            .IsRequired();

        builder.OwnsOne(tp => tp.PublicTransportCost, ptc =>
        {
            ptc.Property(p => p.CostPerPerson)
                .HasColumnName("public_transport_cost_per_person")
                .HasColumnType("decimal(18,2)");

            ptc.Property(p => p.ParticipantCount)
                .HasColumnName("public_transport_participant_count");

            ptc.Property(p => p.TotalAmount)
                .HasColumnName("public_transport_total_amount")
                .HasColumnType("decimal(18,2)");
        });

        builder.Property(tp => tp.TotalCost)
            .HasColumnName("total_cost")
            .HasColumnType("decimal(18,2)");

        builder.HasMany(tp => tp.Vehicles)
            .WithOne()
            .HasForeignKey(v => v.TransportPlanId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(tp => tp.DomainEvents);

        builder.Metadata
            .FindNavigation(nameof(TransportPlan.Vehicles))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
