
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Data;

public sealed class AccountDataUpgrade(
    IDbContextFactory<FinanceDbContext> dbFactory)
{
    public async Task RunAsync()
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        await db.Database.ExecuteSqlRawAsync("""
            UPDATE Accounts
            SET Type = 0, IsActive = 1
            WHERE Name = 'Personal Funds';

            UPDATE Accounts
            SET Type = 1, IsActive = 1
            WHERE Name = 'Company Funds';

            UPDATE Accounts
            SET Type = 1, IsActive = 0
            WHERE Name IN (
                'Company Owes Me',
                'I Owe Company',
                'Expenses',
                'Income'
            );

            UPDATE Accounts
            SET Type = 1, IsActive = 0
            WHERE Type NOT IN (0, 1);
            """);
    }
}
