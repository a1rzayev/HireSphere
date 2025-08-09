using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HireSphere.Core.Models;
using HireSphere.Core.Repositories;
using HireSphere.Infrastructure.ORM;
using HireSphere.Infrastructure.Repositories.Base;

namespace HireSphere.Infrastructure.Repositories;

public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
{
    public CategoryRepository(HireSphereDbContext context) : base(context) { }

    public override async Task<Category?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Category?> GetBySlugAsync(string slug)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.Slug == slug);
    }

    public override async Task<IEnumerable<Category>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public override async Task AddAsync(Category category)
    {
        var existingCategory = await _dbSet.FirstOrDefaultAsync(c => c.Name.ToLower() == category.Name.ToLower());
        if (existingCategory != null)
        {
            throw new InvalidOperationException($"A category with the name '{category.Name}' already exists.");
        }


        var existingSlug = await _dbSet.FirstOrDefaultAsync(c => c.Slug == category.Slug);
        if (existingSlug != null)
        {
            throw new InvalidOperationException($"A category with the slug '{category.Slug}' already exists.");
        }

        await base.AddAsync(category);
    }

    public override async Task UpdateAsync(Category category)
    {

        var existingCategory = await _dbSet
            .FirstOrDefaultAsync(c => 
                c.Name.ToLower() == category.Name.ToLower() && 
                c.Id != category.Id);

        if (existingCategory != null)
        {
            throw new InvalidOperationException($"A category with the name '{category.Name}' already exists.");
        }


        var existingSlug = await _dbSet
            .FirstOrDefaultAsync(c => 
                c.Slug == category.Slug && 
                c.Id != category.Id);

        if (existingSlug != null)
        {
            throw new InvalidOperationException($"A category with the slug '{category.Slug}' already exists.");
        }

        await base.UpdateAsync(category);
    }
}
