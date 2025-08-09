using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HireSphere.Core.Models;
using HireSphere.Core.Filters;

namespace HireSphere.Core.Repositories;

public interface IJobApplicationRepository
{
    Task<JobApplication?> GetByIdAsync(Guid id);
    Task<IEnumerable<JobApplication>> GetByJobIdAsync(Guid jobId);
    Task<IEnumerable<JobApplication>> GetByApplicantUserIdAsync(Guid userId);
    Task<IEnumerable<JobApplication>> SearchAsync(JobApplicationSearchFilter filter);
    Task AddAsync(JobApplication application);
    Task UpdateAsync(JobApplication application);
    Task DeleteAsync(Guid id);
}
