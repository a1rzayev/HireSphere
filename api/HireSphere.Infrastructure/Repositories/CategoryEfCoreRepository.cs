using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HireSphere.Core.Models;
using HireSphere.Infrastructure.ORM;
using HireSphere.Core.Repositories;

namespace HireSphere.Infrastructure.Repositories;

public class CategoryEfCoreRepository : ICategoryEfCoreRepository
{
    private readonly HireSphereDbContext _context;
    private readonly DbSet<Category> _dbSet;

    public CategoryEfCoreRepository(HireSphereDbContext context)
    {
        _context = context;
        _dbSet = context.Set<Category>();
    }

    public async Task<Category?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<IEnumerable<Category>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task AddAsync(Category category)
    {
        await _dbSet.AddAsync(category);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Category category)
    {
        _dbSet.Update(category);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var category = await GetByIdAsync(id);
        if (category != null)
        {
            _dbSet.Remove(category);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<Category?> GetByNameAsync(string name)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower());
    }

    public async Task<Category?> GetBySlugAsync(string slug)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.Slug.ToLower() == slug.ToLower());
    }

    public async Task<IEnumerable<Category>> GetByNameContainsAsync(string name)
    {
        return await _dbSet.Where(c => c.Name.ToLower().Contains(name.ToLower())).ToListAsync();
    }

    public async Task<int> GetTotalCategoriesCountAsync()
    {
        return await _dbSet.CountAsync();
    }
}
