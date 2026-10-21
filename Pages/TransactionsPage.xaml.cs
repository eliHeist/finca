using FinanceTracker.ViewModels;
using FinanceTracker.Services;

namespace FinanceTracker.Pages;

public partial class TransactionsPage : ContentPage
{
    private readonly TransactionsViewModel _viewModel;

    public TransactionsPage(TransactionsViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadAsync();
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        if (sender is not Button { BindingContext: RecentTransaction item })
            return;
    
        var confirmed = await DisplayAlertAsync(
            "Delete transaction",
            $"Delete '{item.Title}'? This action cannot be undone.",
            "Delete",
            "Cancel");
    
        if (confirmed)
            await _viewModel.DeleteCommand.ExecuteAsync(item);
    }
}
