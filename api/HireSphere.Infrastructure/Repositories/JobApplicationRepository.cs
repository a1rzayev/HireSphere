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

public class JobApplicationRepository : BaseRepository<JobApplication>, IJobApplicationRepository
{
    private readonly DbSet<Job> _jobDbSet;
    private readonly DbSet<User> _userDbSet;

    public JobApplicationRepository(HireSphereDbContext context) : base(context)
    {
        _jobDbSet = context.Set<Job>();
        _userDbSet = context.Set<User>();
    }

    public override async Task<JobApplication?> GetByIdAsync(Guid id)
    {
        return await _dbSet
            .FirstOrDefaultAsync(ja => ja.Id == id);
    }

    public async Task<IEnumerable<JobApplication>> GetByJobIdAsync(Guid jobId)
    {
        return await _dbSet
            .Where(ja => ja.JobId == jobId)
            .ToListAsync();
    }

    public async Task<IEnumerable<JobApplication>> GetByApplicantUserIdAsync(Guid userId)
    {
        return await _dbSet
            .Where(ja => ja.ApplicantUserId == userId)
            .ToListAsync();
    }

    public async Task<IEnumerable<JobApplication>> SearchAsync(JobApplicationSearchFilter filter)
    {
        var query = _dbSet.AsQueryable();

        if (filter.JobId.HasValue)
        {
            query = query.Where(ja => ja.JobId == filter.JobId.Value);
        }

        if (filter.ApplicantUserId.HasValue)
        {
            query = query.Where(ja => ja.ApplicantUserId == filter.ApplicantUserId.Value);
        }

        if (filter.Status.HasValue)
        {
            query = query.Where(ja => ja.Status == filter.Status.Value);
        }

        if (filter.AppliedAfter.HasValue)
        {
            query = query.Where(ja => ja.AppliedAt >= filter.AppliedAfter.Value);
        }

        if (filter.AppliedBefore.HasValue)
        {
            query = query.Where(ja => ja.AppliedAt <= filter.AppliedBefore.Value);
        }

        return await query.ToListAsync();
    }

    public override async Task AddAsync(JobApplication application)
    {
        // Validate job exists
        var job = await _jobDbSet.FindAsync(application.JobId);
        if (job == null)
        {
            throw new ArgumentException($"Job with ID {application.JobId} does not exist.");
        }

        // Validate applicant user exists
        var user = await _userDbSet.FindAsync(application.ApplicantUserId);
        if (user == null)
        {
            throw new ArgumentException($"User with ID {application.ApplicantUserId} does not exist.");
        }

        // Check for duplicate application
        var existingApplication = await _dbSet
            .FirstOrDefaultAsync(ja => 
                ja.JobId == application.JobId && 
                ja.ApplicantUserId == application.ApplicantUserId);

        if (existingApplication != null)
        {
            throw new InvalidOperationException("A job application for this job by this user already exists.");
        }

        // Set default values
        application.AppliedAt = DateTime.UtcNow;
        application.Status = JobApplicationStatus.Applied;

        await base.AddAsync(application);
    }

    public override async Task UpdateAsync(JobApplication application)
    {
        // Validate job exists
        var job = await _jobDbSet.FindAsync(application.JobId);
        if (job == null)
        {
            throw new ArgumentException($"Job with ID {application.JobId} does not exist.");
        }

        // Validate applicant user exists
        var user = await _userDbSet.FindAsync(application.ApplicantUserId);
        if (user == null)
        {
            throw new ArgumentException($"User with ID {application.ApplicantUserId} does not exist.");
        }

        await base.UpdateAsync(application);
    }
}
