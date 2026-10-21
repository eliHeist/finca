using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinanceTracker.Services;
using System.Collections.ObjectModel;
using System.Globalization;

namespace FinanceTracker.ViewModels;

public partial class ReportsViewModel(
    ReportService reportService) : ObservableObject
{
    [ObservableProperty]
    private DateTime startDate = new(
        DateTime.Today.Year, DateTime.Today.Month, 1);

    [ObservableProperty]
    private DateTime endDate = DateTime.Today;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private string incomeTotal = "UGX 0";

    [ObservableProperty]
    private string personalExpensesTotal = "UGX 0";

    [ObservableProperty]
    private string externalExpensesTotal = "UGX 0";

    [ObservableProperty]
    private string totalFunds = "UGX 0";

    [ObservableProperty]
    private string personalNetTotal = "UGX 0";

    public ObservableCollection<ExternalFundingDisplay> ExternalFunding { get; } = [];

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
                EndDate);

            IncomeTotal = FormatMoney(report.GenuineIncome);
            PersonalExpensesTotal = FormatMoney(report.PersonalExpenses);
            ExternalExpensesTotal = FormatMoney(report.ExternalExpenses);
            TotalFunds = FormatMoney(report.TotalFunds);
            PersonalNetTotal = FormatMoney(report.PersonalNet);

            ExternalFunding.Clear();

            foreach (var source in report.ExternalFunding)
            {
                ExternalFunding.Add(new ExternalFundingDisplay(
                    source.SourceName,
                    FormatMoney(source.Amount)));
            }
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

public sealed record ExternalFundingDisplay(
    string SourceName,
    string Amount);
