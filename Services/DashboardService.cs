using FinanceTracker.Data;
using FinanceTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Services;

public sealed class DashboardService(
    IDbContextFactory<FinanceDbContext> dbFactory)
{
    public Task<List<RecentTransaction>> GetRecentAsync() =>
        GetTransactionsAsync(10);

    public async Task<List<RecentTransaction>> GetTransactionsAsync(
        int? limit = null)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        var query = db.Transactions
            .AsNoTracking()
            .Where(t => t.IsPosted && !t.IsExternalFundingOffset)
            .OrderByDescending(t => t.OccurredAt);

        var transactions = limit.HasValue
            ? await query.Take(limit.Value).ToListAsync()
            : await query.ToListAsync();

        var categoryIds = transactions
            .Where(t => t.CategoryId.HasValue)
            .Select(t => t.CategoryId!.Value)
            .Distinct()
            .ToList();

        var categories = await db.Categories
            .AsNoTracking()
            .Where(c => categoryIds.Contains(c.Id))
            .ToDictionaryAsync(c => c.Id, c => c.Name);

        var organizations = await db.Organizations
            .AsNoTracking()
            .ToDictionaryAsync(o => o.Id, o => o.Name);

        return transactions.Select(t =>
        {
            var category = t.CategoryId is Guid categoryId &&
                categories.TryGetValue(categoryId, out var categoryName)
                    ? categoryName
                    : t.Kind.ToString();

            var source = t.OrganizationId is Guid organizationId &&
                organizations.TryGetValue(organizationId, out var sourceName)
                    ? sourceName
                    : string.Empty;

            return new RecentTransaction(
                t.Id,
                string.IsNullOrWhiteSpace(t.Description)
                    ? category
                    : t.Description,
                category,
                t.Kind == TransactionKind.Income ? "Income" : "Expense",
                t.Amount,
                t.OccurredAt,
                source);
        }).ToList();
    }
}

public sealed record RecentTransaction(
    Guid Id,
    string Title,
    string Category,
    string Kind,
    long Amount,
    DateTime OccurredAt,
    string SourceName);
