using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HireSphere.Core.Models;
using HireSphere.Core.Enums;
using HireSphere.Core.Filters;

namespace HireSphere.Core.Repositories;

public interface IJobRepository
{
    Task<Job?> GetByIdAsync(Guid id);
    Task<IEnumerable<Job>> GetByCompanyIdAsync(Guid companyId);
    Task<IEnumerable<Job>> SearchAsync(JobSearchFilter filter);
    Task AddAsync(Job job);
    Task UpdateAsync(Job job);
    Task DeleteAsync(Guid id);
}
