using FinanceTracker.Data;
using FinanceTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Services;

public sealed class TransactionService(
    IDbContextFactory<FinanceDbContext> dbFactory)
{
    public async Task<Guid> RecordExpenseAsync(
        long amount,
        Guid categoryId,
        string description,
        DateTime occurredAt,
        bool externallyFunded = false,
        Guid? externalSourceId = null)
    {
        ValidateAmount(amount);

        await using var db = await dbFactory.CreateDbContextAsync();
        await using var dbTransaction =
            await db.Database.BeginTransactionAsync();

        var category = await db.Categories
            .FirstOrDefaultAsync(c =>
                c.Id == categoryId &&
                c.IsActive &&
                c.Kind == TransactionKind.Expense)
            ?? throw new InvalidOperationException(
                "Select a valid expense category.");

        if (externallyFunded && externalSourceId is null)
            throw new ArgumentException(
                "Select the external funding source.");

        if (externalSourceId is Guid sourceId &&
            !await db.Organizations.AnyAsync(o =>
                o.Id == sourceId && o.IsActive))
        {
            throw new InvalidOperationException(
                "The selected funding source is unavailable.");
        }

        var accountType = externallyFunded
            ? AccountType.External
            : AccountType.Personal;

        var account = await db.Accounts
            .FirstOrDefaultAsync(a =>
                a.Type == accountType && a.IsActive)
            ?? throw new InvalidOperationException(
                $"The {accountType} account was not found.");

        var expense = new FinanceTransaction
        {
            Kind = TransactionKind.Expense,
            Amount = amount,
            OccurredAt = occurredAt,
            Description = description.Trim(),
            SourceAccountId = account.Id,
            CategoryId = category.Id,
            OrganizationId = externalSourceId,
            IsPosted = true
        };

        db.Transactions.Add(expense);

        if (externallyFunded)
        {
            var offset = new FinanceTransaction
            {
                Kind = TransactionKind.Income,
                Amount = amount,
                OccurredAt = occurredAt,
                Description = $"External funding offset: {description.Trim()}",
                DestinationAccountId = account.Id,
                OrganizationId = externalSourceId,
                IsExternalFundingOffset = true,
                IsPosted = true
            };

            expense.RelatedTransactionId = offset.Id;
            offset.RelatedTransactionId = expense.Id;

            db.Transactions.Add(offset);
        }

        await db.SaveChangesAsync();
        await dbTransaction.CommitAsync();

        return expense.Id;
    }

    public async Task<Guid> RecordIncomeAsync(
        long amount,
        Guid categoryId,
        string description,
        DateTime occurredAt)
    {
        ValidateAmount(amount);

        await using var db = await dbFactory.CreateDbContextAsync();

        var category = await db.Categories
            .FirstOrDefaultAsync(c =>
                c.Id == categoryId &&
                c.IsActive &&
                c.Kind == TransactionKind.Income)
            ?? throw new InvalidOperationException(
                "Select a valid income category.");

        var personalAccount = await db.Accounts
            .FirstOrDefaultAsync(a =>
                a.Type == AccountType.Personal && a.IsActive)
            ?? throw new InvalidOperationException(
                "The Personal account was not found.");

        var income = new FinanceTransaction
        {
            Kind = TransactionKind.Income,
            Amount = amount,
            OccurredAt = occurredAt,
            Description = description.Trim(),
            DestinationAccountId = personalAccount.Id,
            CategoryId = category.Id,
            IsPosted = true
        };

        db.Transactions.Add(income);
        await db.SaveChangesAsync();

        return income.Id;
    }

    private static void ValidateAmount(long amount)
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(amount),
                "Amount must be greater than zero.");
    }
}
