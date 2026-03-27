using Calcolb.Modules.Transport.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Calcolb.Modules.Transport.Infrastructure.Persistence.Configurations;

public sealed class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.ToTable("transport_vehicles");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(v => v.TransportPlanId)
            .HasColumnName("transport_plan_id")
            .IsRequired();

        builder.ComplexProperty(v => v.PassengerCount, pc =>
        {
            pc.Property(p => p.Value)
                .HasColumnName("passenger_count")
                .IsRequired();
        });

        builder.ComplexProperty(v => v.FuelType, ft =>
        {
            ft.Property(f => f.Value)
                .HasColumnName("fuel_type")
                .HasMaxLength(20)
                .IsRequired();
        });

        builder.ComplexProperty(v => v.FuelConsumption, fc =>
        {
            fc.Property(f => f.Value)
                .HasColumnName("fuel_consumption")
                .HasColumnType("decimal(6,2)")
                .IsRequired();
        });

        builder.HasOne(v => v.RouteDestination)
            .WithOne()
            .HasForeignKey<RouteDestination>(rd => rd.VehicleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
