using FinanceTracker.Models;
using FinanceTracker.ViewModels;

using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;

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

    private async void OnOptionsClicked(object? sender, EventArgs e)
    {
        if (sender is not Button
            {
                CommandParameter: CategoryRow category
            })
        {
            return;
        }
    
        var popup = new OptionsPopup(
            "Category options",
            new[]
            {
                new PopupOption("rename", "Rename category"),
                new PopupOption(
                    "toggle",
                    category.IsActive
                        ? "Deactivate category"
                        : "Activate category")
            });
    
        var result = await this.ShowPopupAsync<string>(popup);
    
        switch (result.Result)
        {
            case "rename":
                await RenameCategoryAsync(category);
                break;
    
            case "toggle":
                await ToggleCategoryActiveAsync(category);
                break;
        }
    }
    
    private async Task RenameCategoryAsync(CategoryRow category)
    {
        var name = await DisplayPromptAsync(
            "Rename category",
            "Enter the new category name:",
            initialValue: category.Name,
            maxLength: 60);
    
        if (name is null)
            return;
    
        await _viewModel.RenameAsync(category.Id, name);
    }
    
    private async Task ToggleCategoryActiveAsync(CategoryRow category)
    {
        var activate = !category.IsActive;
    
        var confirmed = await DisplayAlertAsync(
            activate ? "Activate category" : "Deactivate category",
            activate
                ? $"Activate '{category.Name}'?"
                : $"Deactivate '{category.Name}'?",
            activate ? "Activate" : "Deactivate",
            "Cancel");
    
        if (confirmed)
            await _viewModel.SetActiveAsync(category, activate);
    }

    private async void OnBackClicked(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("..", animate: true);
    }
}
