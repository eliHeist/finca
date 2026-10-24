using FinanceTracker.ViewModels;

namespace FinanceTracker.Pages;

public partial class IncomePage : ContentPage
{
    private readonly IncomeViewModel _viewModel;

    public IncomePage(IncomeViewModel viewModel)
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

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..", animate: true);
    }
}
