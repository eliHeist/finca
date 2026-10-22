namespace FinanceTracker.Models;

public sealed class Category
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = string.Empty;

    public TransactionKind Kind { get; set; }

    public bool IsActive { get; set; } = true;
}

public enum TransactionKind
{
    Expense,
    Income,
    Transfer,
    Allowance,
    Settlement,
    Adjustment
}
