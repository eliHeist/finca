
using CommunityToolkit.Maui.Views;

namespace FinanceTracker.Pages;

public partial class TransactionOptionsPopup : Popup<bool>
{
    public TransactionOptionsPopup()
    {
        InitializeComponent();
    }

    private async void OnDeleteClicked(object? sender, EventArgs e)
    {
        await CloseAsync(true);
    }

    private async void OnCancelClicked(object? sender, EventArgs e)
    {
        await CloseAsync(false);
    }
}
