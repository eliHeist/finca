using FinanceTracker.Data;
using FinanceTracker.Models;
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
            .OrderBy(a => a.Type)
            .ToListAsync();

        var transactions = await db.Transactions
            .AsNoTracking()
            .Where(t => t.IsPosted)
            .ToListAsync();

        var results = new List<AccountBalance>();

        foreach (var account in accounts)
        {
            long amount;

            if (account.Type == AccountType.Personal)
            {
                var income = transactions
                    .Where(t =>
                        t.Kind == TransactionKind.Income &&
                        t.DestinationAccountId == account.Id &&
                        !t.IsExternalFundingOffset)
                    .Sum(t => t.Amount);

                var expenses = transactions
                    .Where(t =>
                        t.Kind == TransactionKind.Expense &&
                        t.SourceAccountId == account.Id)
                    .Sum(t => t.Amount);

                amount = income - expenses;
            }
            else
            {
                amount = transactions
                    .Where(t =>
                        t.Kind == TransactionKind.Expense &&
                        t.SourceAccountId == account.Id)
                    .Sum(t => t.Amount);
            }

            results.Add(new AccountBalance(
                account.Id,
                account.Name,
                account.Type,
                amount));
        }

        return results;
    }
}

public sealed record AccountBalance(
    Guid Id,
    string Name,
    AccountType Type,
    long Balance);
