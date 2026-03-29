using Calcolb.Modules.Expense.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Calcolb.Modules.Expense.Infrastructure.Persistence.Configurations;

public sealed class ExpensePlanConfiguration : IEntityTypeConfiguration<ExpensePlan>
{
    public void Configure(EntityTypeBuilder<ExpensePlan> builder)
    {
        builder.ToTable("expense_plans");

        builder.HasKey(ep => ep.Id);

        builder.Property(ep => ep.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(ep => ep.EventId)
            .HasColumnName("event_id")
            .IsRequired();

        builder.Property(ep => ep.BufferEnabled)
            .HasColumnName("buffer_enabled")
            .IsRequired();

        builder.Property(ep => ep.BufferPercentage)
            .HasColumnName("buffer_percentage")
            .HasColumnType("decimal(5,2)")
            .IsRequired();

        builder.Property(ep => ep.TotalCost)
            .HasColumnName("total_cost")
            .HasColumnType("decimal(18,2)");

        builder.HasMany(ep => ep.ExpenseItems)
            .WithOne()
            .HasForeignKey(ei => ei.ExpensePlanId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(ep => ep.DomainEvents);

        builder.Metadata
            .FindNavigation(nameof(ExpensePlan.ExpenseItems))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
