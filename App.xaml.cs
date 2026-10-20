using FinanceTracker.Data;

namespace FinanceTracker;

public partial class App : Application
{
    private readonly DatabaseInitializer _databaseInitializer;
    private readonly DefaultDataSeeder _defaultDataSeeder;

    public App(
        DatabaseInitializer databaseInitializer,
        DefaultDataSeeder defaultDataSeeder)
    {
        InitializeComponent();

        _databaseInitializer = databaseInitializer;
        _defaultDataSeeder = defaultDataSeeder;
    }

    protected override Window CreateWindow(
        IActivationState? activationState)
    {
        var window = new Window(new ContentPage
        {
            Content = new VerticalStackLayout
            {
                VerticalOptions = LayoutOptions.Center,
                Children =
                {
                    new ActivityIndicator
                    {
                        IsRunning = true,
                        HorizontalOptions = LayoutOptions.Center
                    },
                    new Label
                    {
                        Text = "Preparing your finances...",
                        HorizontalOptions = LayoutOptions.Center
                    }
                }
            }
        });

        InitializeDatabaseAsync(window);

        return window;
    }

    private async void InitializeDatabaseAsync(Window window)
    {
        try
        {
            await _databaseInitializer.InitializeAsync();
            await _defaultDataSeeder.SeedAsync();

            window.Page = new AppShell();
        }
        catch (Exception ex)
        {
            if (window.Page is Page page)
            {
                await page.DisplayAlertAsync(
                    "Database error",
                    ex.Message,
                    "OK");
            }
        }
    }
}