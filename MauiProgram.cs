using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

using FinanceTracker.Data;
using FinanceTracker.Services;

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
        builder.Services.AddTransient<AccountService>();

        return builder.Build();
	}
}
