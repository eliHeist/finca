using FinanceTracker.ViewModels;

namespace FinanceTracker.Pages;

public partial class ExpensePage : ContentPage
{
    private readonly ExpenseViewModel _viewModel;

    public ExpensePage(ExpenseViewModel viewModel)
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
}
