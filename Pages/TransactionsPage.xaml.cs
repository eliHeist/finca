using FinanceTracker.Services;
using FinanceTracker.ViewModels;

using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;

namespace FinanceTracker.Pages;

public partial class TransactionsPage : ContentPage
{
    private readonly TransactionsViewModel _viewModel;

    public TransactionsPage(TransactionsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadAsync();
    }

    private async void OnOptionsClicked(object? sender, EventArgs e)
    {
        if (sender is not Button
            {
                CommandParameter: RecentTransaction item
            })
        {
            return;
        }
    
        var popup = new OptionsPopup(
            "Transaction options",
            new[]
            {
                new PopupOption(
                    "delete",
                    "Delete transaction",
                    IsDestructive: true)
            });
    
        var result = await this.ShowPopupAsync<string>(popup);
    
        if (result.Result != "delete")
            return;
    
        var confirmed = await DisplayAlertAsync(
            "Delete transaction",
            $"Delete '{item.Title}'? This action cannot be undone.",
            "Delete",
            "Cancel");
    
        if (!confirmed)
            return;
    
        await _viewModel.DeleteCommand.ExecuteAsync(item);
    }

    private async Task DeleteTransactionAsync(RecentTransaction item)
    {
        var confirmed = await DisplayAlertAsync(
            "Delete transaction",
            $"Delete '{item.Title}'? This action cannot be undone.",
            "Delete",
            "Cancel");

        if (!confirmed)
            return;

        await _viewModel.DeleteCommand.ExecuteAsync(item);
    }

    private async void OnBackClicked(object? sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("..", animate: true);
    }
}
