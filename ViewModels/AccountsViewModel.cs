
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanceTracker.Models;
using FinanceTracker.Services;
using System.Collections.ObjectModel;
using System.Globalization;

namespace FinanceTracker.ViewModels;

public partial class AccountsViewModel(
    AccountService accountService) : ObservableObject
{
    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    public ObservableCollection<AccountRow> Accounts { get; } = [];

    [RelayCommand]
    public async Task LoadAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            var balances = await accountService.GetBalancesAsync();

            Accounts.Clear();

            foreach (var account in balances)
            {
                var label = account.Type switch
                {
                    AccountType.Personal => "Income minus personal expenses",
                    AccountType.External => "Total externally funded expenses",
                    _ => "Activity"
                };

                Accounts.Add(new AccountRow(
                    account.Id,
                    account.Name,
                    account.Type.ToString(),
                    label,
                    FormatMoney(account.Balance),
                    account.IsActive));
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load accounts: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task RenameAsync(Guid id, string name)
    {
        try
        {
            ErrorMessage = string.Empty;
            await accountService.RenameAsync(id, name);
            await LoadAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }

    public async Task SetActiveAsync(AccountRow account, bool isActive)
    {
        try
        {
            ErrorMessage = string.Empty;
            await accountService.SetActiveAsync(account.Id, isActive);
            await LoadAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }

    private static string FormatMoney(long amount) =>
        $"UGX {amount.ToString("N0", CultureInfo.InvariantCulture)}";
}

public sealed record AccountRow(
    Guid Id,
    string Name,
    string Type,
    string Description,
    string Balance,
    bool IsActive)
{
    public string StatusAction => IsActive ? "Deactivate" : "Activate";
}
