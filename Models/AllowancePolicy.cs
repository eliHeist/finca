
namespace FinanceTracker.Models;

public sealed class AllowancePolicy
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid OrganizationId { get; set; }

    public long DailyRate { get; set; }

    public DateOnly EffectiveFrom { get; set; }

    public DateOnly? EffectiveUntil { get; set; }

    public bool IsActive { get; set; } = true;
}
