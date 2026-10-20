using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanceTracker.Services;
using System.Globalization;

namespace FinanceTracker.ViewModels;

public partial class ReportsViewModel(
    ReportService reportService) : ObservableObject
{
    [ObservableProperty] private DateTime startDate = new(
        DateTime.Today.Year, DateTime.Today.Month, 1);

    [ObservableProperty] private DateTime endDate = DateTime.Today;
    [ObservableProperty] private string dailyAllowance = "10000";
    [ObservableProperty] private string eligibleDays = "0";
    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private string errorMessage = string.Empty;

    [ObservableProperty] private string incomeTotal = "UGX 0";
    [ObservableProperty] private string personalExpensesTotal = "UGX 0";
    [ObservableProperty] private string externalExpensesTotal = "UGX 0";
    [ObservableProperty] private string allowanceEarnedTotal = "UGX 0";
    [ObservableProperty] private string personalNetTotal = "UGX 0";
    [ObservableProperty] private string allowancePositionTotal = "UGX 0";
    [ObservableProperty] private string eligibleDaysLabel = "0 days";

    [RelayCommand]
    public async Task LoadAsync()
    {
        StartDate = new DateTime(
            DateTime.Today.Year,
            DateTime.Today.Month,
            1);

        EndDate = DateTime.Today;

        await RefreshAsync();
    }

    [RelayCommand]
    public async Task RefreshAsync()
    {
        if (IsBusy)
            return;

        ErrorMessage = string.Empty;

        if (!long.TryParse(
                DailyAllowance,
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out var rate) || rate < 0)
        {
            ErrorMessage = "Enter a valid daily allowance.";
            return;
        }

        if (!int.TryParse(
                EligibleDays,
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out var days) || days < 0)
        {
            ErrorMessage = "Enter a valid number of eligible days.";
            return;
        }

        if (StartDate.Date > EndDate.Date)
        {
            ErrorMessage = "Start date must be on or before end date.";
            return;
        }

        try
        {
            IsBusy = true;

            var report = await reportService.GetReportAsync(
                StartDate,
                EndDate,
                rate,
                days);

            IncomeTotal = FormatMoney(report.GenuineIncome);
            PersonalExpensesTotal = FormatMoney(report.PersonalExpenses);
            ExternalExpensesTotal = FormatMoney(report.ExternalExpenses);
            AllowanceEarnedTotal = FormatMoney(report.AllowanceEarned);
            PersonalNetTotal = FormatMoney(report.PersonalNet);
            AllowancePositionTotal = FormatMoney(report.AllowancePosition);
            EligibleDaysLabel = $"{report.EligibleDays} days";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Unable to load report: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private static string FormatMoney(long amount) =>
        $"UGX {amount.ToString("N0", CultureInfo.InvariantCulture)}";
}
