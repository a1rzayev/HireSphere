
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using HireSphere.Core.Models;
using HireSphere.Core.Enums;
using HireSphere.Core.Repositories;

namespace HireSphere.Infrastructure.Services;

public class UserService
{
    private readonly IUserEfCoreRepository _userRepository;

    public UserService(IUserEfCoreRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task RegisterUserAsync(User user, string plainPassword)
    {
        if (!IsPasswordComplex(plainPassword))
            throw new ArgumentException("Password does not meet complexity requirements.");
        user.GetType().GetProperty("PasswordHash")?.SetValue(user, HashPassword(plainPassword));
        await _userRepository.AddAsync(user);
    }

    public async Task ChangeEmailAsync(Guid userId, string newEmail)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new ArgumentException("User not found.");
        if (string.IsNullOrWhiteSpace(newEmail) || !IsValidEmail(newEmail))
            throw new ArgumentException("Invalid email format.");
        user.GetType().GetProperty("Email")?.SetValue(user, newEmail);
        user.GetType().GetProperty("IsEmailConfirmed")?.SetValue(user, false);
        await _userRepository.UpdateAsync(user);
    }

    public async Task ChangePasswordAsync(Guid userId, string newPassword)
    {
        if (!IsPasswordComplex(newPassword))
            throw new ArgumentException("Password does not meet complexity requirements.");
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new ArgumentException("User not found.");
        user.GetType().GetProperty("PasswordHash")?.SetValue(user, HashPassword(newPassword));
        await _userRepository.UpdateAsync(user);
    }

    public async Task<User?> GetUserByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user ?? throw new ArgumentException("User not found.");
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        return user ?? throw new ArgumentException("User not found.");
    }

    private bool IsPasswordComplex(string password)
    {
        if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
            return false;
        if (!Regex.IsMatch(password, @"[A-Z]")) return false;
        if (!Regex.IsMatch(password, @"[a-z]")) return false;
        if (!Regex.IsMatch(password, @"[0-9]")) return false;
        if (!Regex.IsMatch(password, @"[!@#$%^&*(),.?""{}]")) return false;

        return true;
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

    private string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
