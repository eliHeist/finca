
using FinanceTracker.Data;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Services;

public sealed class AccountService(
    IDbContextFactory<FinanceDbContext> dbFactory)
{
    public async Task<List<AccountBalance>> GetBalancesAsync()
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        var accounts = await db.Accounts
            .AsNoTracking()
            .Where(a => a.IsActive)
            .OrderBy(a => a.Name)
            .ToListAsync();

        var totals = await db.LedgerEntries
            .AsNoTracking()
            .GroupBy(e => e.AccountId)
            .Select(g => new
            {
                AccountId = g.Key,
                Balance = g.Sum(e => e.AmountDelta)
            })
            .ToDictionaryAsync(x => x.AccountId, x => x.Balance);

        return accounts.Select(account => new AccountBalance(
            account.Id,
            account.Name,
            account.Type,
            totals.GetValueOrDefault(account.Id, 0)
        )).ToList();
    }
}

public sealed record AccountBalance(
    Guid Id,
    string Name,
    Models.AccountType Type,
    long Balance
);
