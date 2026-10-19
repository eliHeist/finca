using Microsoft.Extensions.Logging;
using FinanceTracker.Data;
using Microsoft.EntityFrameworkCore;

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

        return builder.Build();
	}
}
