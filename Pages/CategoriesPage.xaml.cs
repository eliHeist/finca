using FinanceTracker.Models;
using FinanceTracker.ViewModels;

namespace FinanceTracker.Pages;

public partial class CategoriesPage : ContentPage
{
    private readonly CategoriesViewModel _viewModel;

    public CategoriesPage(CategoriesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadAsync();
    }

    private async void OnAddClicked(object? sender, EventArgs e)
    {
        var choice = await DisplayActionSheet(
            "Choose category type",
            "Cancel",
            null,
            "Expense",
            "Income");

        if (choice is not ("Expense" or "Income"))
            return;

        var kind = choice == "Income"
            ? TransactionKind.Income
            : TransactionKind.Expense;

        var name = await DisplayPromptAsync(
            $"Add {choice.ToLower()} category",
            "Enter the category name:",
            maxLength: 100);

        if (!string.IsNullOrWhiteSpace(name))
            await _viewModel.CreateAsync(name, kind);
    }

    private async void OnRenameClicked(object? sender, EventArgs e)
    {
        if (sender is not Button
            {
                CommandParameter: CategoryRow category
            })
            return;

        var name = await DisplayPromptAsync(
            "Rename category",
            "Enter the new name:",
            initialValue: category.Name,
            maxLength: 100);

        if (name is not null)
            await _viewModel.RenameAsync(category.Id, name);
    }

    private async void OnToggleActiveClicked(object? sender, EventArgs e)
    {
        if (sender is not Button
            {
                CommandParameter: CategoryRow category
            })
            return;

        var activate = !category.IsActive;

        var confirmed = await DisplayAlert(
            activate ? "Activate category" : "Deactivate category",
            activate
                ? $"Activate '{category.Name}'?"
                : $"Deactivate '{category.Name}'? Existing transactions will keep their category.",
            activate ? "Activate" : "Deactivate",
            "Cancel");

        if (confirmed)
            await _viewModel.SetActiveAsync(category, activate);
    }

    private async void OnBackClicked(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("//SettingsPage");
    }
}
