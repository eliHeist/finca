using FinanceTracker.Services;
using FinanceTracker.ViewModels;

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

    private async void OnDeleteClicked(object? sender, EventArgs e)
    {
        if (sender is not Button
            {
                CommandParameter: RecentTransaction item
            })
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
}
