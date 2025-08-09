using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HireSphere.Core.Models;
using HireSphere.Core.Enums;
using HireSphere.Core.Repositories;
using HireSphere.Infrastructure.ORM;
using HireSphere.Infrastructure.Repositories.Base;

namespace HireSphere.Infrastructure.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(HireSphereDbContext context) : base(context) { }

    public override async Task<User?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id) ?? null;
    }

    public override async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Email == email) ?? null;
    }

    public override async Task AddAsync(User user)
    {
        if (await _dbSet.AnyAsync(u => u.Email == user.Email))
            throw new ArgumentException("Email must be unique.");

        await base.AddAsync(user);
    }

    public override async Task UpdateAsync(User user)
    {
        if (await _dbSet.AnyAsync(u => u.Email == user.Email && u.Id != user.Id))
            throw new ArgumentException("Email must be unique.");

        await base.UpdateAsync(user);
    }

    private void ValidateUser(User user)
    {
        if (string.IsNullOrWhiteSpace(user.Email) || !IsValidEmail(user.Email))
            throw new ArgumentException("Invalid email format.");
        if (!Enum.IsDefined(typeof(Role), user.Role))
            throw new ArgumentException("Invalid role.");
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}
