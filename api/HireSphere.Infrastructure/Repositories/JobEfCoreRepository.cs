using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HireSphere.Core.Models;
using HireSphere.Infrastructure.ORM;

namespace HireSphere.Infrastructure.Repositories;

public class JobEfCoreRepository
{
    private readonly HireSphereDbContext _context;
    private readonly DbSet<Job> _dbSet;

    public JobEfCoreRepository(HireSphereDbContext context)
    {
        _context = context;
        _dbSet = context.Set<Job>();
    }

    public async Task<Job?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<IEnumerable<Job>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task AddAsync(Job job)
    {
        await _dbSet.AddAsync(job);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Job job)
    {
        _dbSet.Update(job);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var job = await GetByIdAsync(id);
        if (job != null)
        {
            _dbSet.Remove(job);
            await _context.SaveChangesAsync();
        }
    }
}
