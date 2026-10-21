using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanceTracker.Services;
using System.Collections.ObjectModel;
using System.Globalization;

namespace FinanceTracker.ViewModels;

public partial class HomeViewModel(
    ReportService reportService,
    DashboardService dashboardService) : ObservableObject
{
    [ObservableProperty]
    private string totalFunds = "UGX 0";

    [ObservableProperty]
    private string genuineIncome = "UGX 0";

    [ObservableProperty]
    private string personalExpenses = "UGX 0";

    [ObservableProperty]
    private string externalExpenses = "UGX 0";

    [ObservableProperty]
    private string personalNet = "UGX 0";

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    public ObservableCollection<RecentTransaction> RecentTransactions { get; } = [];

    [RelayCommand]
    public async Task LoadAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            var today = DateTime.Today;
            var start = new DateTime(today.Year, today.Month, 1);

            var report = await reportService.GetReportAsync(start, today);
            var recent = await dashboardService.GetRecentAsync();

            TotalFunds = FormatMoney(report.TotalFunds);
            GenuineIncome = FormatMoney(report.GenuineIncome);
            PersonalExpenses = FormatMoney(report.PersonalExpenses);
            ExternalExpenses = FormatMoney(report.ExternalExpenses);
            PersonalNet = FormatMoney(report.PersonalNet);

            RecentTransactions.Clear();

            foreach (var transaction in recent)
                RecentTransactions.Add(transaction);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Unable to load dashboard: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private static string FormatMoney(long amount) =>
        $"UGX {amount.ToString("N0", CultureInfo.InvariantCulture)}";
}
