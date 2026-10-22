using CommunityToolkit.Mvvm.ComponentModel;
using FinanceTracker.Services;
using System.Collections.ObjectModel;

namespace FinanceTracker.ViewModels;

public partial class OrganizationsViewModel(
    OrganizationService organizationService) : ObservableObject
{
    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    public ObservableCollection<OrganizationRow> Organizations { get; }
        = [];

    public async Task LoadAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            var organizations = await organizationService.GetAllAsync();

            Organizations.Clear();

            foreach (var organization in organizations)
            {
                Organizations.Add(new OrganizationRow(
                    organization.Id,
                    organization.Name,
                    organization.IsActive));
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load organizations: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task CreateAsync(string name)
    {
        try
        {
            ErrorMessage = string.Empty;
            await organizationService.CreateAsync(name);
            await LoadAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }

    public async Task RenameAsync(Guid id, string name)
    {
        try
        {
            ErrorMessage = string.Empty;
            await organizationService.RenameAsync(id, name);
            await LoadAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }

    public async Task SetActiveAsync(OrganizationRow organization, bool isActive)
    {
        try
        {
            ErrorMessage = string.Empty;
            await organizationService.SetActiveAsync(
                organization.Id, isActive);

            await LoadAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }
}

public sealed record OrganizationRow(
    Guid Id,
    string Name,
    bool IsActive)
{
    public string StatusAction => IsActive ? "Deactivate" : "Activate";
}
