namespace FinanceTracker.Models;

public sealed class FinanceTransaction
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public TransactionKind Kind { get; set; }

    // Whole UGX shillings
    public long Amount { get; set; }

    public DateTime OccurredAt { get; set; } = DateTime.Now;

    public string Description { get; set; } = string.Empty;

    public Guid? SourceAccountId { get; set; }

    public Guid? DestinationAccountId { get; set; }

    public Guid? CategoryId { get; set; }

    // Identifies the external funding source
    public Guid? OrganizationId { get; set; }

    // Links an expense to its automatically created offset entry
    public Guid? RelatedTransactionId { get; set; }

    public bool IsExternalFundingOffset { get; set; }

    public bool IsPosted { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
