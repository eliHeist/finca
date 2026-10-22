using FinanceTracker.Data;
using FinanceTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Services;

public sealed class CategoryService(
    IDbContextFactory<FinanceDbContext> dbFactory)
{
    private static readonly TransactionKind[] SupportedKinds =
    [
        TransactionKind.Income,
        TransactionKind.Expense
    ];

    public async Task<List<Category>> GetAllAsync()
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        return await db.Categories
            .AsNoTracking()
            .Where(c => SupportedKinds.Contains(c.Kind))
            .OrderBy(c => c.Kind)
            .ThenBy(c => c.Name)
            .ToListAsync();
    }

    public async Task CreateAsync(string name, TransactionKind kind)
    {
        name = ValidateName(name);
        ValidateKind(kind);

        await using var db = await dbFactory.CreateDbContextAsync();

        if (await db.Categories.AnyAsync(c =>
            c.Kind == kind &&
            c.Name.ToLower() == name.ToLower()))
        {
            throw new InvalidOperationException(
                "A category with this name already exists for this transaction type.");
        }

        db.Categories.Add(new Category
        {
            Name = name,
            Kind = kind
        });

        await db.SaveChangesAsync();
    }

    public async Task RenameAsync(Guid id, string name)
    {
        name = ValidateName(name);

        await using var db = await dbFactory.CreateDbContextAsync();

        var category = await db.Categories.FindAsync(id)
            ?? throw new InvalidOperationException("Category not found.");

        if (await db.Categories.AnyAsync(c =>
            c.Id != id &&
            c.Kind == category.Kind &&
            c.Name.ToLower() == name.ToLower()))
        {
            throw new InvalidOperationException(
                "A category with this name already exists for this transaction type.");
        }

        category.Name = name;
        await db.SaveChangesAsync();
    }

    public async Task SetActiveAsync(Guid id, bool isActive)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        var category = await db.Categories.FindAsync(id)
            ?? throw new InvalidOperationException("Category not found.");

        category.IsActive = isActive;
        await db.SaveChangesAsync();
    }

    private static string ValidateName(string name)
    {
        name = name.Trim();

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name is required.");

        if (name.Length > 100)
            throw new ArgumentException(
                "Category name cannot exceed 100 characters.");

        return name;
    }

    private static void ValidateKind(TransactionKind kind)
    {
        if (!SupportedKinds.Contains(kind))
            throw new ArgumentException(
                "Only income and expense categories are supported.");
    }
}
