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
                    account.Name,
                    label,
                    FormatMoney(account.Balance)));
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

    private static string FormatMoney(long amount) =>
        $"UGX {amount.ToString("N0", CultureInfo.InvariantCulture)}";
}

public sealed record AccountRow(
    string Name,
    string Type,
    string Balance);
