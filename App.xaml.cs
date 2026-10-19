using FinanceTracker.Data;

namespace FinanceTracker;

public partial class App : Application
{
    private readonly DatabaseInitializer _databaseInitializer;

    public App(DatabaseInitializer databaseInitializer)
    {
        InitializeComponent();

        _databaseInitializer = databaseInitializer;

        MainPage = new ContentPage
        {
            Content = new VerticalStackLayout
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                Children =
                {
                    new ActivityIndicator
                    {
                        IsRunning = true
                    },
                    new Label
                    {
                        Text = "Preparing your finances..."
                    }
                }
            }
        };

        InitializeDatabaseAsync();
    }

    private async void InitializeDatabaseAsync()
    {
        try
        {
            await _databaseInitializer.InitializeAsync();

            MainPage = new AppShell();
        }
        catch (Exception ex)
        {
            await MainPage!.DisplayAlert(
                "Database error",
                ex.Message,
                "OK"
            );
        }
    }
}
