
namespace FinanceTracker.Models;

public sealed class LedgerEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid TransactionId { get; set; }

    public Guid AccountId { get; set; }

    // Positive increases the account balance.
    // Negative decreases the account balance.
    public long AmountDelta { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
