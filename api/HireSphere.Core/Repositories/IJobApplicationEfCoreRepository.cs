using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HireSphere.Core.Models;

namespace HireSphere.Core.Repositories;

public interface IJobApplicationEfCoreRepository
{
    Task<JobApplication?> GetByIdAsync(Guid id);
    Task<IEnumerable<JobApplication>> GetAllAsync();
    Task AddAsync(JobApplication jobApplication);
    Task UpdateAsync(JobApplication jobApplication);
    Task DeleteAsync(Guid id);
}
