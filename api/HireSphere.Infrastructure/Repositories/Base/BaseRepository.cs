using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HireSphere.Infrastructure.ORM;

namespace HireSphere.Infrastructure.Repositories.Base;

public class BaseRepository<T> where T : class
{
    protected readonly HireSphereDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public BaseRepository(HireSphereDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public virtual async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public virtual async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }

    public virtual async Task DeleteAsync(Guid id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }

    public virtual async Task<T?> GetByEmailAsync(string email)
    {
        // This is a default implementation that will likely be overridden in specific repositories
        // For generic repositories, this might not make sense, so it returns null
        return null;
    }
}
