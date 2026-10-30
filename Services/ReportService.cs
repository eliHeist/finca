
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
        {
            throw new ArgumentException(
                "Start date must be on or before end date.");
        }

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

        // Include inactive accounts for historical reports.
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

        // Include inactive categories for historical reports.
        var categories = await db.Categories
            .AsNoTracking()
            .ToDictionaryAsync(c => c.Id, c => c.Name);

        var sources = await db.Organizations
            .AsNoTracking()
            .ToDictionaryAsync(o => o.Id, o => o.Name);

        // Genuine income credited to Personal accounts.
        var genuineIncomeTransactions = transactions
            .Where(t =>
                t.Kind == TransactionKind.Income &&
                !t.IsExternalFundingOffset &&
                t.DestinationAccountId.HasValue &&
                personalAccountIds.Contains(
                    t.DestinationAccountId.Value))
            .ToList();

        var genuineIncome = genuineIncomeTransactions
            .Sum(t => t.Amount);

        // Personal expenses.
        var personalExpenseTransactions = transactions
            .Where(t =>
                t.Kind == TransactionKind.Expense &&
                t.SourceAccountId.HasValue &&
                personalAccountIds.Contains(t.SourceAccountId.Value))
            .ToList();

        var personalExpenses = personalExpenseTransactions
            .Sum(t => t.Amount);

        // Employer-funded expenses.
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

        // Expense breakdown by category.
        var allExpenseTransactions = personalExpenseTransactions
            .Concat(externalExpenseTransactions)
            .ToList();

        var totalExpenses = personalExpenses + externalExpenses;

        var expenseCategories = allExpenseTransactions
            .GroupBy(t => t.CategoryId)
            .Select(group =>
            {
                var personalAmount = group
                    .Where(t =>
                        t.SourceAccountId.HasValue &&
                        personalAccountIds.Contains(
                            t.SourceAccountId.Value))
                    .Sum(t => t.Amount);

                var employerAmount = group
                    .Where(t =>
                        t.SourceAccountId.HasValue &&
                        externalAccountIds.Contains(
                            t.SourceAccountId.Value))
                    .Sum(t => t.Amount);

                var totalAmount = personalAmount + employerAmount;

                return new ExpenseCategoryRow(
                    GetCategoryName(group.Key, categories),
                    personalAmount,
                    employerAmount,
                    totalAmount,
                    CalculatePercentage(totalAmount, totalExpenses));
            })
            .OrderByDescending(row => row.TotalAmount)
            .ToList();

        // Genuine income breakdown by category.
        var incomeCategories = genuineIncomeTransactions
            .GroupBy(t => t.CategoryId)
            .Select(group =>
            {
                var amount = group.Sum(t => t.Amount);

                return new IncomeCategoryRow(
                    GetCategoryName(group.Key, categories),
                    amount,
                    CalculatePercentage(amount, genuineIncome));
            })
            .OrderByDescending(row => row.Amount)
            .ToList();

        return new FinanceReport(
            from,
            endDate.Date,
            genuineIncome,
            personalExpenses,
            externalExpenses,
            genuineIncome + externalExpenses,
            externalFunding,
            expenseCategories,
            incomeCategories);
    }

    private static string GetCategoryName(
        Guid? categoryId,
        Dictionary<Guid, string> categories)
    {
        if (categoryId.HasValue &&
            categories.TryGetValue(categoryId.Value, out var name))
        {
            return name;
        }

        return "Uncategorised";
    }

    private static double CalculatePercentage(
        long amount,
        long total)
    {
        return total == 0
            ? 0
            : amount * 100d / total;
    }
}

public sealed record ExternalFundingRow(
    string SourceName,
    long Amount);

public sealed record ExpenseCategoryRow(
    string CategoryName,
    long PersonalAmount,
    long EmployerAmount,
    long TotalAmount,
    double Percentage);

public sealed record IncomeCategoryRow(
    string CategoryName,
    long Amount,
    double Percentage);

public sealed record FinanceReport(
    DateTime StartDate,
    DateTime EndDate,
    long GenuineIncome,
    long PersonalExpenses,
    long ExternalExpenses,
    long TotalFunds,
    List<ExternalFundingRow> ExternalFunding,
    List<ExpenseCategoryRow> ExpenseCategories,
    List<IncomeCategoryRow> IncomeCategories)
{
    public long PersonalNet => GenuineIncome - PersonalExpenses;
}
