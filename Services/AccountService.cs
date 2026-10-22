
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
                account.IsActive,
                amount));
        }

        return results;
    }

    public async Task RenameAsync(Guid id, string name)
    {
        name = name.Trim();

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Account name is required.");

        await using var db = await dbFactory.CreateDbContextAsync();

        var account = await db.Accounts.FindAsync(id)
            ?? throw new InvalidOperationException("Account not found.");

        if (await db.Accounts.AnyAsync(a =>
            a.Id != id &&
            a.Name.ToLower() == name.ToLower()))
        {
            throw new InvalidOperationException(
                "An account with this name already exists.");
        }

        account.Name = name;
        await db.SaveChangesAsync();
    }

    public async Task SetActiveAsync(Guid id, bool isActive)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        var account = await db.Accounts.FindAsync(id)
            ?? throw new InvalidOperationException("Account not found.");

        account.IsActive = isActive;
        await db.SaveChangesAsync();
    }
}

public sealed record AccountBalance(
    Guid Id,
    string Name,
    AccountType Type,
    bool IsActive,
    long Balance);
