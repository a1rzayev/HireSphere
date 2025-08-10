using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HireSphere.Core.Models;
using HireSphere.Infrastructure.ORM;
using HireSphere.Core.Repositories;

namespace HireSphere.Infrastructure.Repositories;

public class CompanyEfCoreRepository : ICompanyEfCoreRepository
{
    private readonly HireSphereDbContext _context;
    private readonly DbSet<Company> _dbSet;

    public CompanyEfCoreRepository(HireSphereDbContext context)
    {
        _context = context;
        _dbSet = context.Set<Company>();
    }

    public async Task<Company?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<IEnumerable<Company>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task AddAsync(Company company)
    {
        await _dbSet.AddAsync(company);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Company company)
    {
        _dbSet.Update(company);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var company = await GetByIdAsync(id);
        if (company != null)
        {
            _dbSet.Remove(company);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Company>> GetByOwnerUserIdAsync(Guid ownerUserId)
    {
        return await _dbSet.Where(c => c.OwnerUserId == ownerUserId).ToListAsync();
    }

    public async Task<IEnumerable<Company>> GetByNameAsync(string name)
    {
        return await _dbSet.Where(c => c.Name.ToLower().Contains(name.ToLower())).ToListAsync();
    }

    public async Task<IEnumerable<Company>> GetByLocationAsync(string location)
    {
        return await _dbSet.Where(c => c.Location != null && c.Location.ToLower().Contains(location.ToLower())).ToListAsync();
    }
}
