using Calcolb.Modules.Expense.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Calcolb.Modules.Expense.Infrastructure.Persistence;

public sealed class ExpenseDbContext : DbContext
{
    public DbSet<ExpensePlan> ExpensePlans => Set<ExpensePlan>();
    public DbSet<ExpenseItem> ExpenseItems => Set<ExpenseItem>();

    public ExpenseDbContext(DbContextOptions<ExpenseDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ExpenseDbContext).Assembly);
    }
}
