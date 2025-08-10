using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HireSphere.Core.Models;
using HireSphere.Infrastructure.ORM;
using HireSphere.Core.Repositories;
using HireSphere.Core.Enums;

namespace HireSphere.Infrastructure.Repositories;

public class JobApplicationEfCoreRepository : IJobApplicationEfCoreRepository
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

    public async Task<IEnumerable<JobApplication>> GetByJobIdAsync(Guid jobId)
    {
        return await _dbSet.Where(ja => ja.JobId == jobId).ToListAsync();
    }

    public async Task<IEnumerable<JobApplication>> GetByApplicantUserIdAsync(Guid applicantUserId)
    {
        return await _dbSet.Where(ja => ja.ApplicantUserId == applicantUserId).ToListAsync();
    }

    public async Task<IEnumerable<JobApplication>> GetByStatusAsync(JobApplicationStatus status)
    {
        return await _dbSet.Where(ja => ja.Status == status).ToListAsync();
    }

    public async Task<JobApplication?> GetByJobAndApplicantAsync(Guid jobId, Guid applicantUserId)
    {
        return await _dbSet.FirstOrDefaultAsync(ja => ja.JobId == jobId && ja.ApplicantUserId == applicantUserId);
    }
}
