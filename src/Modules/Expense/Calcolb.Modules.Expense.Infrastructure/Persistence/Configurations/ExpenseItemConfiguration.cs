using Calcolb.Modules.Expense.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Calcolb.Modules.Expense.Infrastructure.Persistence.Configurations;

public sealed class ExpenseItemConfiguration : IEntityTypeConfiguration<ExpenseItem>
{
    public void Configure(EntityTypeBuilder<ExpenseItem> builder)
    {
        builder.ToTable("expense_items");

        builder.HasKey(ei => ei.Id);

        builder.Property(ei => ei.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(ei => ei.ExpensePlanId)
            .HasColumnName("expense_plan_id")
            .IsRequired();

        builder.Property(ei => ei.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(ei => ei.Amount)
            .HasColumnName("amount")
            .HasColumnType("decimal(18,2)")
            .IsRequired();
    }
}
