using FinanceTracker.ViewModels;

namespace FinanceTracker.Pages;

public partial class HomePage : ContentPage
{
    private readonly HomeViewModel _viewModel;

    public HomePage(HomeViewModel viewModel)
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

    private async void OnAddIncomeClicked(
        object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(IncomePage));
    }

    private async void OnAddExpenseClicked(
        object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(ExpensePage));
    }
}
