using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HireSphere.Core.Models;

namespace HireSphere.Core.Repositories;

public interface ICategoryEfCoreRepository
{
    Task<Category?> GetByIdAsync(Guid id);
    Task<IEnumerable<Category>> GetAllAsync();
    Task AddAsync(Category category);
    Task UpdateAsync(Category category);
    Task DeleteAsync(Guid id);
    Task<Category?> GetByNameAsync(string name);
    Task<Category?> GetBySlugAsync(string slug);
    Task<IEnumerable<Category>> GetByNameContainsAsync(string name);
}
