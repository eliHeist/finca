using FinanceTracker.Services;
using FinanceTracker.ViewModels;

using CommunityToolkit.Maui;
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
    
        var popup = new TransactionOptionsPopup();
    
        var result = await this.ShowPopupAsync<bool>(
            popup,
            new PopupOptions
            {
                CanBeDismissedByTappingOutsideOfPopup = true,
                PageOverlayColor = Colors.Transparent,
                Shape = null,
                Shadow = null
            });
    
        if (result.WasDismissedByTappingOutsideOfPopup ||
            result.Result != true)
        {
            return;
        }
    
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
