using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HireSphere.Core.Models;
using HireSphere.Core.Enums;

namespace HireSphere.Core.Repositories;

public interface IJobApplicationEfCoreRepository
{
    Task<JobApplication?> GetByIdAsync(Guid id);
    Task<IEnumerable<JobApplication>> GetAllAsync();
    Task AddAsync(JobApplication jobApplication);
    Task UpdateAsync(JobApplication jobApplication);
    Task DeleteAsync(Guid id);
    Task<IEnumerable<JobApplication>> GetByJobIdAsync(Guid jobId);
    Task<IEnumerable<JobApplication>> GetByApplicantUserIdAsync(Guid applicantUserId);
    Task<IEnumerable<JobApplication>> GetByStatusAsync(JobApplicationStatus status);
    Task<JobApplication?> GetByJobAndApplicantAsync(Guid jobId, Guid applicantUserId);
}
