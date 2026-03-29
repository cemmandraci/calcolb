using Calcolb.Modules.Estimation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Calcolb.Modules.Estimation.Infrastructure.Persistence.Configurations;

public sealed class EstimationSessionConfiguration : IEntityTypeConfiguration<EstimationSession>
{
    public void Configure(EntityTypeBuilder<EstimationSession> builder)
    {
        builder.ToTable("estimation_sessions");

        builder.HasKey(es => es.Id);

        builder.Property(es => es.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(es => es.EventId)
            .HasColumnName("event_id")
            .IsRequired();

        builder.Property(es => es.ParticipantCount)
            .HasColumnName("participant_count")
            .IsRequired();

        builder.Property(es => es.TransportPlanId)
            .HasColumnName("transport_plan_id");

        builder.Property(es => es.ShoppingCartId)
            .HasColumnName("shopping_cart_id");

        builder.Property(es => es.ExpensePlanId)
            .HasColumnName("expense_plan_id");

        builder.Property(es => es.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .IsRequired();

        builder.Property(es => es.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(es => es.TransportCost)
            .HasColumnName("transport_cost")
            .HasColumnType("decimal(18,2)");

        builder.Property(es => es.ShoppingCost)
            .HasColumnName("shopping_cost")
            .HasColumnType("decimal(18,2)");

        builder.Property(es => es.ExpenseCost)
            .HasColumnName("expense_cost")
            .HasColumnType("decimal(18,2)");

        builder.OwnsOne(es => es.Summary, summary =>
        {
            summary.Property(s => s.TotalTransportCost)
                .HasColumnName("summary_transport_cost")
                .HasColumnType("decimal(18,2)");

            summary.Property(s => s.TotalShoppingCost)
                .HasColumnName("summary_shopping_cost")
                .HasColumnType("decimal(18,2)");

            summary.Property(s => s.TotalExpenseCost)
                .HasColumnName("summary_expense_cost")
                .HasColumnType("decimal(18,2)");

            summary.Property(s => s.BufferAmount)
                .HasColumnName("summary_buffer_amount")
                .HasColumnType("decimal(18,2)");

            summary.Property(s => s.GrandTotal)
                .HasColumnName("summary_grand_total")
                .HasColumnType("decimal(18,2)");

            summary.Property(s => s.CostPerPerson)
                .HasColumnName("summary_cost_per_person")
                .HasColumnType("decimal(18,2)");
        });

        builder.Ignore(es => es.DomainEvents);
    }
}
