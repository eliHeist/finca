using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanceTracker.Services;
using System.Collections.ObjectModel;
using System.Globalization;

namespace FinanceTracker.ViewModels;

public partial class TransactionsViewModel(
    DashboardService dashboardService,
    TransactionService transactionService) : ObservableObject
{
    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    public ObservableCollection<RecentTransaction> Transactions { get; } = [];

    [RelayCommand]
    public async Task LoadAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            var items = await dashboardService.GetTransactionsAsync();

            Transactions.Clear();

            foreach (var item in items)
                Transactions.Add(item);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Unable to load transactions: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task DeleteAsync(RecentTransaction? item)
    {
        if (item is null || IsBusy)
            return;

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            await transactionService.DeleteTransactionAsync(item.Id);
            Transactions.Remove(item);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Unable to delete transaction: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    public static string FormatAmount(long amount) =>
        $"UGX {amount.ToString("N0", CultureInfo.InvariantCulture)}";
}
