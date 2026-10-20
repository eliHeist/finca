using FinanceTracker.Data;
using FinanceTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Services;

public sealed class ReportService(
    IDbContextFactory<FinanceDbContext> dbFactory)
{
    public async Task<FinanceReport> GetReportAsync(
        DateTime startDate,
        DateTime endDate,
        long dailyAllowance,
        int eligibleDays)
    {
        if (startDate.Date > endDate.Date)
            throw new ArgumentException(
                "Start date must be on or before end date.");

        if (dailyAllowance < 0)
            throw new ArgumentOutOfRangeException(
                nameof(dailyAllowance));

        if (eligibleDays < 0)
            throw new ArgumentOutOfRangeException(
                nameof(eligibleDays));

        await using var db = await dbFactory.CreateDbContextAsync();

        var accounts = await db.Accounts
            .AsNoTracking()
            .ToListAsync();

        var personalAccountIds = accounts
            .Where(a => a.Type == AccountType.Personal)
            .Select(a => a.Id)
            .ToHashSet();

        var externalAccountIds = accounts
            .Where(a => a.Type == AccountType.External)
            .Select(a => a.Id)
            .ToHashSet();

        var from = startDate.Date;
        var until = endDate.Date.AddDays(1);

        var transactions = await db.Transactions
            .AsNoTracking()
            .Where(t =>
                t.IsPosted &&
                t.OccurredAt >= from &&
                t.OccurredAt < until)
            .ToListAsync();

        var genuineIncome = transactions
            .Where(t =>
                t.Kind == TransactionKind.Income &&
                !t.IsExternalFundingOffset)
            .Sum(t => t.Amount);

        var personalExpenses = transactions
            .Where(t =>
                t.Kind == TransactionKind.Expense &&
                t.SourceAccountId.HasValue &&
                personalAccountIds.Contains(t.SourceAccountId.Value))
            .Sum(t => t.Amount);

        var externalExpenses = transactions
            .Where(t =>
                t.Kind == TransactionKind.Expense &&
                t.SourceAccountId.HasValue &&
                externalAccountIds.Contains(t.SourceAccountId.Value))
            .Sum(t => t.Amount);

        return new FinanceReport(
            from,
            endDate.Date,
            genuineIncome,
            personalExpenses,
            externalExpenses,
            dailyAllowance * eligibleDays,
            eligibleDays);
    }
}

public sealed record FinanceReport(
    DateTime StartDate,
    DateTime EndDate,
    long GenuineIncome,
    long PersonalExpenses,
    long ExternalExpenses,
    long AllowanceEarned,
    int EligibleDays)
{
    public long PersonalNet =>
        GenuineIncome - PersonalExpenses;

    public long AllowancePosition =>
        AllowanceEarned - PersonalExpenses;
}
