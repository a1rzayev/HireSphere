using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HireSphere.Core.Models;
using HireSphere.Core.Enums;
using HireSphere.Core.Repositories;
using HireSphere.Core.Filters;
using HireSphere.Infrastructure.ORM;
using HireSphere.Infrastructure.Repositories.Base;

namespace HireSphere.Infrastructure.Repositories;

public class JobRepository : BaseRepository<Job>, IJobRepository
{
    private readonly DbSet<Company> _companyDbSet;
    private readonly DbSet<Category> _categoryDbSet;

    public JobRepository(HireSphereDbContext context) : base(context)
    {
        _companyDbSet = context.Set<Company>();
        _categoryDbSet = context.Set<Category>();
    }

    public override async Task<Job?> GetByIdAsync(Guid id)
    {
        return await _dbSet
            .FirstOrDefaultAsync(j => j.Id == id);
    }

    public async Task<IEnumerable<Job>> GetByCompanyIdAsync(Guid companyId)
    {
        return await _dbSet
            .Where(j => j.CompanyId == companyId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Job>> SearchAsync(JobSearchFilter filter)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Title))
        {
            query = query.Where(j => j.Title.Contains(filter.Title, StringComparison.OrdinalIgnoreCase));
        }

        if (filter.CompanyId.HasValue)
        {
            query = query.Where(j => j.CompanyId == filter.CompanyId.Value);
        }

        if (filter.CategoryId.HasValue)
        {
            query = query.Where(j => j.CategoryId == filter.CategoryId.Value);
        }

        if (filter.JobType.HasValue)
        {
            query = query.Where(j => j.JobType == filter.JobType.Value);
        }

        if (filter.IsRemote.HasValue)
        {
            query = query.Where(j => j.IsRemote == filter.IsRemote.Value);
        }

        if (filter.MinSalary.HasValue)
        {
            query = query.Where(j => 
                (j.SalaryFrom.HasValue && j.SalaryFrom.Value >= filter.MinSalary.Value) ||
                (j.SalaryTo.HasValue && j.SalaryTo.Value >= filter.MinSalary.Value));
        }

        if (filter.MaxSalary.HasValue)
        {
            query = query.Where(j => 
                (j.SalaryFrom.HasValue && j.SalaryFrom.Value <= filter.MaxSalary.Value) ||
                (j.SalaryTo.HasValue && j.SalaryTo.Value <= filter.MaxSalary.Value));
        }

        if (filter.IsActive.HasValue)
        {
            query = query.Where(j => j.IsActive == filter.IsActive.Value);
        }

        return await query.ToListAsync();
    }

    public override async Task AddAsync(Job job)
    {
        var company = await _companyDbSet.FindAsync(job.CompanyId);
        if (company == null)
        {
            throw new ArgumentException($"Company with ID {job.CompanyId} does not exist.");
        }

        var category = await _categoryDbSet.FindAsync(job.CategoryId);
        if (category == null)
        {
            throw new ArgumentException($"Category with ID {job.CategoryId} does not exist.");
        }

        job.PostedAt = DateTime.UtcNow;
        if (job.ExpiresAt < job.PostedAt)
        {
            job.ExpiresAt = job.PostedAt.AddDays(30);
        }

        await base.AddAsync(job);
    }

    public override async Task UpdateAsync(Job job)
    {
        var company = await _companyDbSet.FindAsync(job.CompanyId);
        if (company == null)
        {
            throw new ArgumentException($"Company with ID {job.CompanyId} does not exist.");
        }

        var category = await _categoryDbSet.FindAsync(job.CategoryId);
        if (category == null)
        {
            throw new ArgumentException($"Category with ID {job.CategoryId} does not exist.");
        }

        await base.UpdateAsync(job);
    }
}
