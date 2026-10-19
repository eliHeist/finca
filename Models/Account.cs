
namespace FinanceTracker.Models;

public sealed class Account
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = string.Empty;

    public AccountType Type { get; set; }

    public Guid? OrganizationId { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}

public enum AccountType
{
    PersonalFunds,
    CompanyFunds,
    Receivable,
    Payable,
    ExternalSource
}
