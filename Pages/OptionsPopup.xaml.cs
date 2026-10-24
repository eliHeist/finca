using CommunityToolkit.Maui.Views;

namespace FinanceTracker.Pages;

public sealed record PopupOption(
    string Key,
    string Label,
    bool IsDestructive = false);

public partial class OptionsPopup : Popup<string>
{
    public OptionsPopup(
        string title,
        IEnumerable<PopupOption> options)
    {
        InitializeComponent();

        PopupTitle.Text = title.ToUpperInvariant();

        var primaryColor = GetResourceColor("TextPrimary");
        var errorColor = GetResourceColor("Error");
        var primaryColorBg = GetResourceColor("Secondary");
        var errorColorBg = GetResourceColor("ErrorBg");

        foreach (var option in options)
        {
            var button = new Button
            {
                Text = option.Label,
                BackgroundColor = option.IsDestructive
                    ? errorColorBg
                    : primaryColorBg,
                TextColor = option.IsDestructive
                    ? errorColor
                    : primaryColor,
                FontSize = 14,
                HorizontalOptions = LayoutOptions.Fill,
                HorizontalTextAlignment = TextAlignment.Start,
                Padding = new Thickness(14, 12),
                CornerRadius = 12,
                CommandParameter = option.Key
            };

            button.Clicked += OnOptionClicked;
            OptionsLayout.Children.Add(button);
        }
    }

    private static Color GetResourceColor(string key)
    {
        if (Application.Current?.Resources.TryGetValue(
                key, out var value) == true
            && value is Color color)
        {
            return color;
        }

        return Colors.White;
    }

    private async void OnOptionClicked(object? sender, EventArgs e)
    {
        if (sender is Button { CommandParameter: string key })
        {
            await CloseAsync(key);
        }
    }

    private async void OnCancelClicked(object? sender, EventArgs e)
    {
        await CloseAsync(string.Empty);
    }
}