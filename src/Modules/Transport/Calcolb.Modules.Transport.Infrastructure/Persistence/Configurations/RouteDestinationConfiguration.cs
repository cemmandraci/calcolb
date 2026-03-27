using Calcolb.Modules.Transport.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Calcolb.Modules.Transport.Infrastructure.Persistence.Configurations;

public sealed class RouteDestinationConfiguration : IEntityTypeConfiguration<RouteDestination>
{
    public void Configure(EntityTypeBuilder<RouteDestination> builder)
    {
        builder.ToTable("transport_route_destinations");

        builder.HasKey(rd => rd.Id);

        builder.Property(rd => rd.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(rd => rd.VehicleId)
            .HasColumnName("vehicle_id")
            .IsRequired();

        builder.Property(rd => rd.Origin)
            .HasColumnName("origin")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(rd => rd.Destination)
            .HasColumnName("destination")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(rd => rd.Distance)
            .HasColumnName("distance")
            .HasColumnType("decimal(10,2)");
    }
}
