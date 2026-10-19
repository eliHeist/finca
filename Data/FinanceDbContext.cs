using FinanceTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Data;

public sealed class FinanceDbContext(
    DbContextOptions<FinanceDbContext> options
) : DbContext(options)
{
    public DbSet<Account> Accounts => Set<Account>();

    public DbSet<Organization> Organizations => Set<Organization>();

    public DbSet<Category> Categories => Set<Category>();

    public DbSet<FinanceTransaction> Transactions =>
        Set<FinanceTransaction>();

    public DbSet<LedgerEntry> LedgerEntries =>
        Set<LedgerEntry>();

    public DbSet<AllowancePolicy> AllowancePolicies =>
        Set<AllowancePolicy>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Account>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<Organization>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<Category>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<FinanceTransaction>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<LedgerEntry>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<AllowancePolicy>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<Account>()
            .HasIndex(x => x.Name);

        modelBuilder.Entity<FinanceTransaction>()
            .HasIndex(x => x.OccurredAt);

        modelBuilder.Entity<LedgerEntry>()
            .HasIndex(x => new
            {
                x.AccountId,
                x.CreatedAtUtc
            });

        modelBuilder.Entity<LedgerEntry>()
            .HasIndex(x => x.TransactionId);
    }
}
