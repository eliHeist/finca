using FinanceTracker.Data;
using FinanceTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceTracker.Services;

public sealed class OrganizationService(
    IDbContextFactory<FinanceDbContext> dbFactory)
{
    public async Task<List<Organization>> GetAllAsync()
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        return await db.Organizations
            .AsNoTracking()
            .OrderBy(o => o.Name)
            .ToListAsync();
    }

    public async Task CreateAsync(string name)
    {
        name = ValidateName(name);

        await using var db = await dbFactory.CreateDbContextAsync();

        if (await NameExistsAsync(db, name))
            throw new InvalidOperationException(
                "An organization with this name already exists.");

        db.Organizations.Add(new Organization
        {
            Name = name
        });

        await db.SaveChangesAsync();
    }

    public async Task RenameAsync(Guid id, string name)
    {
        name = ValidateName(name);

        await using var db = await dbFactory.CreateDbContextAsync();

        var organization = await db.Organizations.FindAsync(id)
            ?? throw new InvalidOperationException(
                "Organization not found.");

        if (await db.Organizations.AnyAsync(o =>
            o.Id != id &&
            o.Name.ToLower() == name.ToLower()))
        {
            throw new InvalidOperationException(
                "An organization with this name already exists.");
        }

        organization.Name = name;

        await db.SaveChangesAsync();
    }

    public async Task SetActiveAsync(Guid id, bool isActive)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

        var organization = await db.Organizations.FindAsync(id)
            ?? throw new InvalidOperationException(
                "Organization not found.");

        organization.IsActive = isActive;

        await db.SaveChangesAsync();
    }

    private static string ValidateName(string name)
    {
        name = name.Trim();

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Organization name is required.");

        if (name.Length > 100)
            throw new ArgumentException(
                "Organization name cannot exceed 100 characters.");

        return name;
    }

    private static Task<bool> NameExistsAsync(
        FinanceDbContext db,
        string name)
    {
        return db.Organizations.AnyAsync(o =>
            o.Name.ToLower() == name.ToLower());
    }
}
