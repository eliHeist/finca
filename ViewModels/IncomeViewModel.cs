using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanceTracker.Data;
using FinanceTracker.Models;
using FinanceTracker.Services;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Globalization;

namespace FinanceTracker.ViewModels;

public partial class IncomeViewModel(
    IDbContextFactory<FinanceDbContext> dbFactory,
    TransactionService transactionService) : ObservableObject
{
    [ObservableProperty] private string amount = string.Empty;
    [ObservableProperty] private string description = string.Empty;
    [ObservableProperty] private DateTime occurredAt = DateTime.Today;
    [ObservableProperty] private Category? selectedCategory;
    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private string statusMessage = string.Empty;

    public ObservableCollection<Category> Categories { get; } = [];

    [RelayCommand]
    public async Task LoadAsync()
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        Categories.Clear();

        var categories = await db.Categories
            .AsNoTracking()
            .Where(c => c.IsActive && c.Kind == TransactionKind.Income)
            .OrderBy(c => c.Name)
            .ToListAsync();

        foreach (var category in categories)
            Categories.Add(category);

        if (SelectedCategory is null ||
            !Categories.Any(c => c.Id == SelectedCategory.Id))
        {
            SelectedCategory = Categories.FirstOrDefault();
        }
    }

    [RelayCommand]
    public async Task SaveAsync()
    {
        if (IsBusy)
            return;

        StatusMessage = string.Empty;

        if (!long.TryParse(
                Amount,
                NumberStyles.AllowThousands,
                CultureInfo.InvariantCulture,
                out var parsedAmount) || parsedAmount <= 0)
        {
            StatusMessage = "Enter a valid amount in UGX.";
            return;
        }

        if (SelectedCategory is null)
        {
            StatusMessage = "Choose an income category.";
            return;
        }

        try
        {
            IsBusy = true;

            await transactionService.RecordIncomeAsync(
                amount: parsedAmount,
                categoryId: SelectedCategory.Id,
                description: Description.Trim(),
                occurredAt: OccurredAt);

            Amount = string.Empty;
            Description = string.Empty;
            OccurredAt = DateTime.Today;
            StatusMessage = "Income saved successfully.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Unable to save income: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
