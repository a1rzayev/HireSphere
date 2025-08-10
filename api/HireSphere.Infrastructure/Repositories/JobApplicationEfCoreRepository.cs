using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HireSphere.Core.Models;
using HireSphere.Infrastructure.ORM;

namespace HireSphere.Infrastructure.Repositories;

public class JobApplicationEfCoreRepository
{
    private readonly HireSphereDbContext _context;
    private readonly DbSet<JobApplication> _dbSet;

    public JobApplicationEfCoreRepository(HireSphereDbContext context)
    {
        _context = context;
        _dbSet = context.Set<JobApplication>();
    }

    public async Task<JobApplication?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<IEnumerable<JobApplication>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task AddAsync(JobApplication jobApplication)
    {
        await _dbSet.AddAsync(jobApplication);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(JobApplication jobApplication)
    {
        _dbSet.Update(jobApplication);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var jobApplication = await GetByIdAsync(id);
        if (jobApplication != null)
        {
            _dbSet.Remove(jobApplication);
            await _context.SaveChangesAsync();
        }
    }
}
