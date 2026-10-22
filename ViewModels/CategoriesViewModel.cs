using CommunityToolkit.Mvvm.ComponentModel;
using FinanceTracker.Models;
using FinanceTracker.Services;
using System.Collections.ObjectModel;

namespace FinanceTracker.ViewModels;

public partial class CategoriesViewModel(
    CategoryService categoryService) : ObservableObject
{
    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    public ObservableCollection<CategoryRow> Categories { get; } = [];

    public async Task LoadAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            var categories = await categoryService.GetAllAsync();

            Categories.Clear();

            foreach (var category in categories)
            {
                Categories.Add(new CategoryRow(
                    category.Id,
                    category.Name,
                    category.Kind,
                    category.IsActive));
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load categories: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task CreateAsync(string name, TransactionKind kind)
    {
        try
        {
            ErrorMessage = string.Empty;
            await categoryService.CreateAsync(name, kind);
            await LoadAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }

    public async Task RenameAsync(Guid id, string name)
    {
        try
        {
            ErrorMessage = string.Empty;
            await categoryService.RenameAsync(id, name);
            await LoadAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }

    public async Task SetActiveAsync(CategoryRow category, bool isActive)
    {
        try
        {
            ErrorMessage = string.Empty;
            await categoryService.SetActiveAsync(category.Id, isActive);
            await LoadAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }
}

public sealed record CategoryRow(
    Guid Id,
    string Name,
    TransactionKind Kind,
    bool IsActive)
{
    public string KindLabel => Kind == TransactionKind.Income
        ? "Income"
        : "Expense";

    public string StatusAction => IsActive ? "Deactivate" : "Activate";
}
