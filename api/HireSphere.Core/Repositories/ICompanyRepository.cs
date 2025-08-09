using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HireSphere.Core.Models;

namespace HireSphere.Core.Repositories;

public interface ICompanyRepository
{
    Task<Company?> GetByIdAsync(Guid id);
    Task<IEnumerable<Company>> GetByOwnerUserIdAsync(Guid userId);
    Task AddAsync(Company company);
    Task UpdateAsync(Company company);
    Task DeleteAsync(Guid id);
}
