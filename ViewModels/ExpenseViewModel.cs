using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanceTracker.Data;
using FinanceTracker.Models;
using FinanceTracker.Services;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Globalization;

namespace FinanceTracker.ViewModels;

public partial class ExpenseViewModel(
    IDbContextFactory<FinanceDbContext> dbFactory,
    TransactionService transactionService) : ObservableObject
{
    [ObservableProperty] private string amount = string.Empty;
    [ObservableProperty] private string description = string.Empty;
    [ObservableProperty] private DateTime occurredAt = DateTime.Today;
    [ObservableProperty] private bool isExternallyFunded;
    [ObservableProperty] private Category? selectedCategory;
    [ObservableProperty] private Organization? selectedSource;
    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private string statusMessage = string.Empty;

    public ObservableCollection<Category> Categories { get; } = [];
    public ObservableCollection<Organization> Sources { get; } = [];

    [RelayCommand]
    public async Task LoadAsync()
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        Categories.Clear();
        Sources.Clear();

        var categories = await db.Categories
            .AsNoTracking()
            .Where(c => c.IsActive && c.Kind == TransactionKind.Expense)
            .OrderBy(c => c.Name)
            .ToListAsync();

        var sources = await db.Organizations
            .AsNoTracking()
            .Where(o => o.IsActive)
            .OrderBy(o => o.Name)
            .ToListAsync();

        foreach (var category in categories)
            Categories.Add(category);

        foreach (var source in sources)
            Sources.Add(source);

        SelectedCategory ??= Categories.FirstOrDefault();
        SelectedSource ??= Sources.FirstOrDefault();
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
            StatusMessage = "Choose an expense category.";
            return;
        }

        if (IsExternallyFunded && SelectedSource is null)
        {
            StatusMessage = "Choose the external funding source.";
            return;
        }

        try
        {
            IsBusy = true;

            await transactionService.RecordExpenseAsync(
                amount: parsedAmount,
                categoryId: SelectedCategory.Id,
                description: Description.Trim(),
                occurredAt: OccurredAt,
                externallyFunded: IsExternallyFunded,
                externalSourceId: IsExternallyFunded
                    ? SelectedSource!.Id
                    : null);

            Amount = string.Empty;
            Description = string.Empty;
            OccurredAt = DateTime.Today;
            IsExternallyFunded = false;
            StatusMessage = "Expense saved successfully.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Unable to save expense: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
            await Shell.Current.GoToAsync("//HomePage");
        }
    }
}
