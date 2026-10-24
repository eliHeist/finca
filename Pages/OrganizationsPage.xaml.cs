using FinanceTracker.ViewModels;

using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;

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

    private async void OnOptionsClicked(object? sender, EventArgs e)
    {
        if (sender is not Button
            {
                CommandParameter: OrganizationRow organization
            })
        {
            return;
        }
    
        var popup = new OptionsPopup(
            "Funding source options",
            new[]
            {
                new PopupOption("rename", "Rename organization"),
                new PopupOption(
                    "toggle",
                    organization.IsActive
                        ? "Deactivate organization"
                        : "Activate organization")
            });
    
        var result = await this.ShowPopupAsync<string>(popup);
    
        switch (result.Result)
        {
            case "rename":
                await RenameOrganizationAsync(organization);
                break;
    
            case "toggle":
                await ToggleOrganizationActiveAsync(organization);
                break;
        }
    }
    
    private async Task RenameOrganizationAsync(
        OrganizationRow organization)
    {
        var name = await DisplayPromptAsync(
            "Rename organization",
            "Enter the new organization name:",
            initialValue: organization.Name,
            maxLength: 60);
    
        if (name is null)
            return;
    
        await _viewModel.RenameAsync(organization.Id, name);
    }
    
    private async Task ToggleOrganizationActiveAsync(
        OrganizationRow organization)
    {
        var activate = !organization.IsActive;
    
        var confirmed = await DisplayAlertAsync(
            activate ? "Activate organization" : "Deactivate organization",
            activate
                ? $"Activate '{organization.Name}'?"
                : $"Deactivate '{organization.Name}'?",
            activate ? "Activate" : "Deactivate",
            "Cancel");
    
        if (confirmed)
            await _viewModel.SetActiveAsync(organization, activate);
    }

    private async void OnBackClicked(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("..", animate: true);
    }
}
