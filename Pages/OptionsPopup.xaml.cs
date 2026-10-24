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

        foreach (var option in options)
        {
            var button = new Button
            {
                Text = option.Label,
                BackgroundColor = Colors.Transparent,
                TextColor = option.IsDestructive
                    ? Color.FromArgb("#D92D20")
                    : Color.FromArgb("#17232F"),
                FontSize = 14,
                HorizontalOptions = LayoutOptions.Fill,
                Padding = new Thickness(12, 10),
                CornerRadius = 10,
                CommandParameter = option.Key
            };

            button.Clicked += OnOptionClicked;
            OptionsLayout.Children.Add(button);
        }
    }

    private async void OnOptionClicked(object? sender, EventArgs e)
    {
        if (sender is Button { CommandParameter: string key })
            await CloseAsync(key);
    }

    private async void OnCancelClicked(object? sender, EventArgs e)
    {
        await CloseAsync(string.Empty);
    }
}