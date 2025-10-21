using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HireSphere.Core.Models;
using HireSphere.Infrastructure.ORM;
using HireSphere.Core.Repositories;

namespace HireSphere.Infrastructure.Repositories;

public class JobEfCoreRepository : IJobEfCoreRepository
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

    public async Task<IEnumerable<Job>> GetByCompanyIdAsync(Guid companyId)
    {
        return await _dbSet.Where(j => j.CompanyId == companyId).ToListAsync();
    }

    public async Task<IEnumerable<Job>> GetByCategoryIdAsync(Guid categoryId)
    {
        return await _dbSet.Where(j => j.CategoryId == categoryId).ToListAsync();
    }

    public async Task<IEnumerable<Job>> GetActiveJobsAsync()
    {
        return await _dbSet.Where(j => j.IsActive && j.ExpiresAt > DateTime.UtcNow).ToListAsync();
    }

    public async Task<IEnumerable<Job>> GetRecentActiveJobsAsync(int count)
    {
        return await _dbSet
            .Where(j => j.IsActive && j.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(j => j.PostedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<Job>> GetFeaturedJobsAsync(int count)
    {
        // For now, return recent active jobs as featured
        // You can implement custom logic for featured jobs later
        return await _dbSet
            .Where(j => j.IsActive && j.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(j => j.PostedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IEnumerable<Job>> GetActiveJobsWithFiltersAsync(int page, int pageSize, string? search, string? location, string? jobType, Guid? categoryId, bool? isRemote)
    {
        var query = _dbSet
            .Where(j => j.IsActive && j.ExpiresAt > DateTime.UtcNow);

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(j => j.Title.Contains(search) || j.Description.Contains(search));
        }

        if (!string.IsNullOrEmpty(location))
        {
            query = query.Where(j => j.Location != null && j.Location.Contains(location));
        }

        if (!string.IsNullOrEmpty(jobType))
        {
            query = query.Where(j => j.JobType.ToString() == jobType);
        }

        if (categoryId.HasValue)
        {
            query = query.Where(j => j.CategoryId == categoryId.Value);
        }

        if (isRemote.HasValue)
        {
            query = query.Where(j => j.IsRemote == isRemote.Value);
        }

        return await query
            .OrderByDescending(j => j.PostedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetTotalActiveJobsCountAsync()
    {
        return await _dbSet.CountAsync(j => j.IsActive && j.ExpiresAt > DateTime.UtcNow);
    }

    public async Task<int> GetActiveJobsCountAsync()
    {
        return await _dbSet.CountAsync(j => j.IsActive && j.ExpiresAt > DateTime.UtcNow);
    }
}
