using FinanceTracker.ViewModels;

namespace FinanceTracker.Pages;

public partial class OrganizationsPage : ContentPage
{
    private readonly OrganizationsViewModel _viewModel;

    public OrganizationsPage(OrganizationsViewModel viewModel)
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
        var name = await DisplayPromptAsync(
            "Add organization",
            "Enter the organization name:",
            maxLength: 100);

        if (!string.IsNullOrWhiteSpace(name))
            await _viewModel.CreateAsync(name);
    }

    private async void OnRenameClicked(object? sender, EventArgs e)
    {
        if (sender is not Button
            {
                CommandParameter: OrganizationRow organization
            })
            return;

        var name = await DisplayPromptAsync(
            "Rename organization",
            "Enter the new name:",
            initialValue: organization.Name,
            maxLength: 100);

        if (name is not null)
            await _viewModel.RenameAsync(organization.Id, name);
    }

    private async void OnToggleActiveClicked(object? sender, EventArgs e)
    {
        if (sender is not Button
            {
                CommandParameter: OrganizationRow organization
            })
            return;

        var activate = !organization.IsActive;

        var confirmed = await DisplayAlert(
            activate ? "Activate organization" : "Deactivate organization",
            activate
                ? $"Activate '{organization.Name}'?"
                : $"Deactivate '{organization.Name}'? It will no longer be available for new funding records, but existing transactions will remain.",
            activate ? "Activate" : "Deactivate",
            "Cancel");

        if (confirmed)
            await _viewModel.SetActiveAsync(organization, activate);
    }
}
