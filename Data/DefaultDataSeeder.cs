using FinanceTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Data;

public sealed class DefaultDataSeeder(
    IDbContextFactory<FinanceDbContext> dbFactory)
{
    public async Task SeedAsync()
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        var categories = new[]
        {
            (Name: "Food", Kind: TransactionKind.Expense),
            (Name: "Transport", Kind: TransactionKind.Expense),
            (Name: "Shopping", Kind: TransactionKind.Expense),
            (Name: "Bills", Kind: TransactionKind.Expense),
            (Name: "Other Expense", Kind: TransactionKind.Expense),
            (Name: "Salary", Kind: TransactionKind.Income),
            (Name: "Allowance", Kind: TransactionKind.Income),
            (Name: "Other Income", Kind: TransactionKind.Income)
        };

        foreach (var item in categories)
        {
            var exists = await db.Categories.AnyAsync(
                c => c.Name == item.Name && c.Kind == item.Kind);

            if (!exists)
            {
                db.Categories.Add(new Category
                {
                    Name = item.Name,
                    Kind = item.Kind
                });
            }
        }

        await db.SaveChangesAsync();
    }
}
