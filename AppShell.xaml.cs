using FinanceTracker.Pages;

namespace FinanceTracker;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(IncomePage), typeof(IncomePage));
        Routing.RegisterRoute(nameof(ExpensePage), typeof(ExpensePage));
        Routing.RegisterRoute(nameof(TransactionsPage), typeof(TransactionsPage));
        Routing.RegisterRoute(nameof(AccountsPage), typeof(AccountsPage));
        Routing.RegisterRoute(nameof(OrganizationsPage), typeof(OrganizationsPage));
        Routing.RegisterRoute(nameof(CategoriesPage), typeof(CategoriesPage));
    }
}
