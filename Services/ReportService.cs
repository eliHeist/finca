using FinanceTracker.Data;
using FinanceTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Services;

public sealed class ReportService(
    IDbContextFactory<FinanceDbContext> dbFactory)
{
    public async Task<FinanceReport> GetReportAsync(
        DateTime startDate,
        DateTime endDate)
    {
        if (startDate.Date > endDate.Date)
            throw new ArgumentException(
                "Start date must be on or before end date.");

        await using var db = await dbFactory.CreateDbContextAsync();

        var from = startDate.Date;
        var until = endDate.Date.AddDays(1);

        var transactions = await db.Transactions
            .AsNoTracking()
            .Where(t =>
                t.IsPosted &&
                t.OccurredAt >= from &&
                t.OccurredAt < until)
            .ToListAsync();

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

        var sources = await db.Organizations
            .AsNoTracking()
            .ToDictionaryAsync(o => o.Id, o => o.Name);

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

        var externalExpenseTransactions = transactions
            .Where(t =>
                t.Kind == TransactionKind.Expense &&
                t.SourceAccountId.HasValue &&
                externalAccountIds.Contains(t.SourceAccountId.Value))
            .ToList();

        var externalFunding = externalExpenseTransactions
            .GroupBy(t => t.OrganizationId)
            .Select(group => new ExternalFundingRow(
                group.Key is Guid sourceId &&
                    sources.TryGetValue(sourceId, out var name)
                        ? name
                        : "Unspecified source",
                group.Sum(t => t.Amount)))
            .OrderByDescending(row => row.Amount)
            .ToList();

        var externalExpenses = externalFunding.Sum(row => row.Amount);

        return new FinanceReport(
            from,
            endDate.Date,
            genuineIncome,
            personalExpenses,
            externalExpenses,
            genuineIncome + externalExpenses,
            externalFunding);
    }
}

public sealed record ExternalFundingRow(
    string SourceName,
    long Amount);

public sealed record FinanceReport(
    DateTime StartDate,
    DateTime EndDate,
    long GenuineIncome,
    long PersonalExpenses,
    long ExternalExpenses,
    long TotalFunds,
    List<ExternalFundingRow> ExternalFunding)
{
    public long PersonalNet => GenuineIncome - PersonalExpenses;
}
