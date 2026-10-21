using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

using FinanceTracker.Data;
using FinanceTracker.Services;
using FinanceTracker.Pages;
using FinanceTracker.ViewModels;

namespace FinanceTracker;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

        var databasePath = Path.Combine(
            FileSystem.AppDataDirectory,
            "ft.db"
        );

        builder.Services.AddDbContextFactory<FinanceDbContext>(
            options => options.UseSqlite(
                $"Data Source={databasePath}"
            )
        );

        builder.Services.AddSingleton<DatabaseInitializer>();
        builder.Services.AddSingleton<DefaultDataSeeder>();
        builder.Services.AddTransient<AccountService>();
        builder.Services.AddTransient<AccountsViewModel>();
        builder.Services.AddTransient<AccountsPage>();
        builder.Services.AddTransient<TransactionService>();
        builder.Services.AddSingleton<AccountDataUpgrade>();
        builder.Services.AddTransient<ExpenseViewModel>();
        builder.Services.AddTransient<ExpensePage>();
        builder.Services.AddTransient<IncomeViewModel>();
        builder.Services.AddTransient<IncomePage>();
        builder.Services.AddTransient<ReportService>();
        builder.Services.AddTransient<ReportsViewModel>();
        builder.Services.AddTransient<ReportsPage>();
        builder.Services.AddTransient<DashboardService>();
        builder.Services.AddTransient<HomeViewModel>();
        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTransient<TransactionsViewModel>();
        builder.Services.AddTransient<TransactionsPage>();

        return builder.Build();
	}
}
