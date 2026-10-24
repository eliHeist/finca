
namespace FinanceTracker.Pages;

public partial class SettingsPage : ContentPage
{
    public SettingsPage()
    {
        InitializeComponent();
    }

    private async void OnAccountsTapped(object? sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(AccountsPage));
    }

    private async void OnSourcesTapped(object? sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(OrganizationsPage));
    }

    private async void OnCategoriesTapped(object? sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(CategoriesPage));
    }
}
