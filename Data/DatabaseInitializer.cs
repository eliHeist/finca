
using FinanceTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Data;

public sealed class DatabaseInitializer(
    IDbContextFactory<FinanceDbContext> dbFactory)
{
    public async Task InitializeAsync()
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        await db.Database.EnsureCreatedAsync();

        if (await db.Accounts.AnyAsync())
            return;

        var organization = new Organization
        {
            Name = "My Employer"
        };

        var accounts = new[]
        {
            new Account
            {
                Name = "Personal Funds",
                Type = AccountType.PersonalFunds
            },
            new Account
            {
                Name = "Company Funds",
                Type = AccountType.CompanyFunds,
                OrganizationId = organization.Id
            },
            new Account
            {
                Name = "Company Owes Me",
                Type = AccountType.Receivable,
                OrganizationId = organization.Id
            },
            new Account
            {
                Name = "I Owe Company",
                Type = AccountType.Payable,
                OrganizationId = organization.Id
            }
        };

        await using var transaction =
            await db.Database.BeginTransactionAsync();

        db.Organizations.Add(organization);
        db.Accounts.AddRange(accounts);

        await db.SaveChangesAsync();
        await transaction.CommitAsync();
    }
}
