using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using HireSphere.Core.Enums;

namespace HireSphere.Core.Models;

public class User
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [StringLength(100, MinimumLength = 5, ErrorMessage = "Email must be between 5 and 100 characters")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "Password hash is required")]
    public required string PasswordHash { get; set; }

    [Required(ErrorMessage = "Role is required")]
    public Role Role { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 50 characters")]
    [RegularExpression(@"^[a-zA-Z\s'-]+$", ErrorMessage = "Name can only contain letters, spaces, hyphens, and apostrophes")]
    public required string Name { get; set; }

    [Required(ErrorMessage = "Surname is required")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Surname must be between 2 and 50 characters")]
    [RegularExpression(@"^[a-zA-Z\s'-]+$", ErrorMessage = "Surname can only contain letters, spaces, hyphens, and apostrophes")]
    public required string Surname { get; set; }

    [Phone(ErrorMessage = "Invalid phone number format")]
    [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
    public string? Phone { get; set; }

    public bool IsEmailConfirmed { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public User() 
    {
        Email = string.Empty;
        PasswordHash = string.Empty;
        Name = string.Empty;
        Surname = string.Empty;
        Role = Role.JobSeeker; //Default role
        CreatedAt = DateTime.UtcNow;
    }

    public User(Guid id, string email, string passwordHash, Role role, string name, string surname, string? phone = null, bool isEmailConfirmed = false, DateTime? createdAt = null)
    {
        Id = id;
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
        Name = name;
        Surname = surname;
        Phone = phone;
        IsEmailConfirmed = isEmailConfirmed;
        CreatedAt = createdAt ?? DateTime.UtcNow;
 
        ValidateUser();
    }

    private void ValidateUser()
    {
        if (!Enum.IsDefined(typeof(Role), Role))
        {
            throw new ArgumentException("Invalid user role.");
        }

        if (!string.IsNullOrWhiteSpace(Phone) && !IsValidPhoneNumber(Phone))
        {
            throw new ArgumentException("Invalid phone number format.");
        }
    }

    private bool IsValidPhoneNumber(string phoneNumber)
    {
        return Regex.IsMatch(phoneNumber, @"^\+?(\d[\d-. ]+)?(\([\d-. ]+\))?[\d-. ]+\d$");
    }

    public void UpdateEmail(string newEmail)
    {
        if (string.IsNullOrWhiteSpace(newEmail))
        {
            throw new ArgumentException("Email cannot be empty.");
        }

        if (newEmail.Length < 5 || newEmail.Length > 100)
        {
            throw new ArgumentException("Email must be between 5 and 100 characters.");
        }

        if (!Regex.IsMatch(newEmail, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            throw new ArgumentException("Invalid email format.");
        }

        Email = newEmail;
        IsEmailConfirmed = false; 
    }
}

