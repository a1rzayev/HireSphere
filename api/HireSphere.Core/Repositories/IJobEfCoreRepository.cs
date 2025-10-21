using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HireSphere.Core.Models;

namespace HireSphere.Core.Repositories;

public interface IJobEfCoreRepository
{
    Task<Job?> GetByIdAsync(Guid id);
    Task<IEnumerable<Job>> GetAllAsync();
    Task AddAsync(Job job);
    Task UpdateAsync(Job job);
    Task DeleteAsync(Guid id);
    Task<IEnumerable<Job>> GetByCompanyIdAsync(Guid companyId);
    Task<IEnumerable<Job>> GetByCategoryIdAsync(Guid categoryId);
    Task<IEnumerable<Job>> GetActiveJobsAsync();
    Task<IEnumerable<Job>> GetRecentActiveJobsAsync(int count);
    Task<IEnumerable<Job>> GetFeaturedJobsAsync(int count);
    Task<IEnumerable<Job>> GetActiveJobsWithFiltersAsync(int page, int pageSize, string? search, string? location, string? jobType, Guid? categoryId, bool? isRemote);
    Task<int> GetTotalActiveJobsCountAsync();
    Task<int> GetActiveJobsCountAsync();
}
