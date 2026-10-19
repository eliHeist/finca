using Microsoft.Extensions.DependencyInjection;

namespace FinanceTracker;

public partial class App : Application
{
	public App()
	{
	    AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
        {
            var ex = args.ExceptionObject as Exception;
            System.Diagnostics.Debug.WriteLine($"CRASH: {ex?.Message} \n {ex?.StackTrace}");
        };
	
        TaskScheduler.UnobservedTaskException += (sender, args) =>
        {
            System.Diagnostics.Debug.WriteLine($"TASK CRASH: {args.Exception?.Message}");
        };
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new AppShell());
	}
}