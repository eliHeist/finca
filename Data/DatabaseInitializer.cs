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

        var employer = new Organization
        {
            Name = "My Employer"
        };

        db.Organizations.Add(employer);

        db.Accounts.AddRange(
            new Account
            {
                Name = "Personal",
                Type = AccountType.Personal
            },
            new Account
            {
                Name = "External",
                Type = AccountType.External,
                OrganizationId = employer.Id
            }
        );

        await db.SaveChangesAsync();
    }
}
