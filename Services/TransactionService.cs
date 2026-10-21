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
    
    public async Task UpdateExpenseAsync(
        Guid transactionId,
        long amount,
        Guid categoryId,
        string description,
        DateTime occurredAt,
        bool externallyFunded,
        Guid? externalSourceId)
    {
        ValidateAmount(amount);
    
        await using var db = await dbFactory.CreateDbContextAsync();
        await using var dbTransaction =
            await db.Database.BeginTransactionAsync();
    
        var expense = await db.Transactions.FirstOrDefaultAsync(t =>
            t.Id == transactionId &&
            t.Kind == TransactionKind.Expense)
            ?? throw new InvalidOperationException("Expense not found.");
    
        var category = await db.Categories.FirstOrDefaultAsync(c =>
            c.Id == categoryId &&
            c.IsActive &&
            c.Kind == TransactionKind.Expense)
            ?? throw new InvalidOperationException("Select a valid expense category.");
    
        if (externallyFunded && externalSourceId is null)
            throw new ArgumentException("Select the external funding source.");
    
        if (externallyFunded &&
            !await db.Organizations.AnyAsync(o =>
                o.Id == externalSourceId && o.IsActive))
        {
            throw new InvalidOperationException("The funding source is unavailable.");
        }
    
        var accountType = externallyFunded
            ? AccountType.External
            : AccountType.Personal;
    
        var account = await db.Accounts.FirstOrDefaultAsync(a =>
            a.Type == accountType && a.IsActive)
            ?? throw new InvalidOperationException(
                $"The {accountType} account was not found.");
    
        expense.Amount = amount;
        expense.CategoryId = category.Id;
        expense.Description = description.Trim();
        expense.OccurredAt = occurredAt;
        expense.SourceAccountId = account.Id;
        expense.OrganizationId = externallyFunded ? externalSourceId : null;
    
        FinanceTransaction? offset = null;
    
        if (expense.RelatedTransactionId is Guid offsetId)
        {
            offset = await db.Transactions.FirstOrDefaultAsync(t =>
                t.Id == offsetId &&
                t.IsExternalFundingOffset);
        }
    
        if (externallyFunded)
        {
            if (offset is null)
            {
                offset = new FinanceTransaction
                {
                    Kind = TransactionKind.Income,
                    IsExternalFundingOffset = true,
                    IsPosted = true,
                    RelatedTransactionId = expense.Id
                };
    
                db.Transactions.Add(offset);
                expense.RelatedTransactionId = offset.Id;
            }
    
            offset.Amount = amount;
            offset.OccurredAt = occurredAt;
            offset.Description = $"External funding offset: {description.Trim()}";
            offset.DestinationAccountId = account.Id;
            offset.OrganizationId = externalSourceId;
            offset.IsPosted = true;
            offset.RelatedTransactionId = expense.Id;
        }
        else
        {
            if (offset is not null)
                db.Transactions.Remove(offset);
    
            expense.RelatedTransactionId = null;
        }
    
        await db.SaveChangesAsync();
        await dbTransaction.CommitAsync();
    }
    
    public async Task UpdateIncomeAsync(
        Guid transactionId,
        long amount,
        Guid categoryId,
        string description,
        DateTime occurredAt)
    {
        ValidateAmount(amount);
    
        await using var db = await dbFactory.CreateDbContextAsync();
    
        var income = await db.Transactions.FirstOrDefaultAsync(t =>
            t.Id == transactionId &&
            t.Kind == TransactionKind.Income &&
            !t.IsExternalFundingOffset)
            ?? throw new InvalidOperationException("Income not found.");
    
        var category = await db.Categories.FirstOrDefaultAsync(c =>
            c.Id == categoryId &&
            c.IsActive &&
            c.Kind == TransactionKind.Income)
            ?? throw new InvalidOperationException("Select a valid income category.");
    
        var personalAccount = await db.Accounts.FirstOrDefaultAsync(a =>
            a.Type == AccountType.Personal && a.IsActive)
            ?? throw new InvalidOperationException("Personal account not found.");
    
        income.Amount = amount;
        income.CategoryId = category.Id;
        income.Description = description.Trim();
        income.OccurredAt = occurredAt;
        income.DestinationAccountId = personalAccount.Id;
    
        await db.SaveChangesAsync();
    }
    
    public async Task DeleteTransactionAsync(Guid transactionId)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        await using var dbTransaction =
            await db.Database.BeginTransactionAsync();
    
        var transaction = await db.Transactions.FirstOrDefaultAsync(t =>
            t.Id == transactionId)
            ?? throw new InvalidOperationException("Transaction not found.");
    
        if (transaction.IsExternalFundingOffset)
        {
            throw new InvalidOperationException(
                "Delete the associated expense instead of its funding offset.");
        }
    
        if (transaction.Kind == TransactionKind.Expense &&
            transaction.RelatedTransactionId is Guid offsetId)
        {
            var offset = await db.Transactions.FirstOrDefaultAsync(t =>
                t.Id == offsetId &&
                t.IsExternalFundingOffset);
    
            if (offset is not null)
                db.Transactions.Remove(offset);
        }
    
        db.Transactions.Remove(transaction);
    
        await db.SaveChangesAsync();
        await dbTransaction.CommitAsync();
    }


    private static void ValidateAmount(long amount)
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(amount),
                "Amount must be greater than zero.");
    }
}
