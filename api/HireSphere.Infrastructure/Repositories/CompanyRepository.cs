using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HireSphere.Core.Models;
using HireSphere.Core.Enums;
using HireSphere.Core.Repositories;
using HireSphere.Infrastructure.ORM;
using HireSphere.Infrastructure.Repositories.Base;

namespace HireSphere.Infrastructure.Repositories;

public class CompanyRepository : BaseRepository<Company>, ICompanyRepository
{
    private readonly DbSet<User> _userDbSet;

    public CompanyRepository(HireSphereDbContext context) : base(context)
    {
        _userDbSet = context.Set<User>();
    }

    public override async Task<Company?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<IEnumerable<Company>> GetByOwnerUserIdAsync(Guid userId)
    {
        return await _dbSet
            .Where(c => c.OwnerUserId == userId)
            .ToListAsync();
    }

    public override async Task AddAsync(Company company)
    {
        // Validate owner user exists and has Employer role
        var ownerUser = await _userDbSet
            .FirstOrDefaultAsync(u => u.Id == company.OwnerUserId && u.Role == Role.Employer);

        if (ownerUser == null)
        {
            throw new ArgumentException("Invalid owner user. Must be an existing Employer.");
        }

        // Validate company data
        ValidateCompany(company);

        // Set creation timestamp
        company.CreatedAt = DateTime.UtcNow;

        await base.AddAsync(company);
    }

    public override async Task UpdateAsync(Company company)
    {
        // Validate owner user exists and has Employer role
        var ownerUser = await _userDbSet
            .FirstOrDefaultAsync(u => u.Id == company.OwnerUserId && u.Role == Role.Employer);

        if (ownerUser == null)
        {
            throw new ArgumentException("Invalid owner user. Must be an existing Employer.");
        }

        // Validate company data
        ValidateCompany(company);

        await base.UpdateAsync(company);
    }

    private void ValidateCompany(Company company)
    {
        // Validate name (already handled by [Required] attribute)
        if (string.IsNullOrWhiteSpace(company.Name))
        {
            throw new ArgumentException("Company name is required.");
        }

        if (company.Name.Length < 2 || company.Name.Length > 100)
        {
            throw new ArgumentException("Company name must be between 2 and 100 characters.");
        }

        // Optional website validation
        if (!string.IsNullOrWhiteSpace(company.Website))
        {
            if (!Uri.TryCreate(company.Website, UriKind.Absolute, out _))
            {
                throw new ArgumentException("Invalid website URL format.");
            }
        }

        // Optional logo URL validation
        if (!string.IsNullOrWhiteSpace(company.LogoUrl))
        {
            if (!Uri.TryCreate(company.LogoUrl, UriKind.Absolute, out _))
            {
                throw new ArgumentException("Invalid logo URL format.");
            }
        }
    }
}
