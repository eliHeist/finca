
using FinanceTracker.ViewModels;

namespace FinanceTracker.Pages;

public partial class AccountsPage : ContentPage
{
    private readonly AccountsViewModel _viewModel;

    public AccountsPage(AccountsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadAsync();
    }

    private async void OnRenameClicked(object sender, EventArgs e)
    {
        if (sender is not Button
            {
                CommandParameter: AccountRow account
            })
            return;

        var name = await DisplayPromptAsync(
            "Rename account",
            "Enter the new account name:",
            initialValue: account.Name,
            maxLength: 60);

        if (name is null)
            return;

        await _viewModel.RenameAsync(account.Id, name);
    }

    private async void OnToggleActiveClicked(object sender, EventArgs e)
    {
        if (sender is not Button
            {
                CommandParameter: AccountRow account
            })
            return;

        var activate = !account.IsActive;

        var confirmed = await DisplayAlertAsync(
            $"{(activate ? "Activate" : "Deactivate")} account",
            activate
                ? $"Activate '{account.Name}'?"
                : $"Deactivate '{account.Name}'? Recording new transactions of this type will be unavailable while the account is inactive.",
            activate ? "Activate" : "Deactivate",
            "Cancel");

        if (confirmed)
            await _viewModel.SetActiveAsync(account, activate);
    }

    private async void OnBackClicked(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("..", animate: true);
    }
}
