
namespace FinanceTracker.Models;

public sealed class FinanceTransaction
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public TransactionKind Kind { get; set; }

    public long Amount { get; set; }

    public DateTime OccurredAt { get; set; } = DateTime.Now;

    public string Description { get; set; } = string.Empty;

    public Guid? SourceAccountId { get; set; }

    public Guid? DestinationAccountId { get; set; }

    public Guid? CategoryId { get; set; }

    public Guid? OrganizationId { get; set; }

    public bool IsPosted { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
