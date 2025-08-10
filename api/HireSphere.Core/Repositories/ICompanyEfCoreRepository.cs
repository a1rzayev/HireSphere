using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HireSphere.Core.Models;

namespace HireSphere.Core.Repositories;

public interface ICompanyEfCoreRepository
{
    Task<Company?> GetByIdAsync(Guid id);
    Task<IEnumerable<Company>> GetAllAsync();
    Task AddAsync(Company company);
    Task UpdateAsync(Company company);
    Task DeleteAsync(Guid id);
    Task<IEnumerable<Company>> GetByOwnerUserIdAsync(Guid ownerUserId);
    Task<IEnumerable<Company>> GetByNameAsync(string name);
    Task<IEnumerable<Company>> GetByLocationAsync(string location);
}
