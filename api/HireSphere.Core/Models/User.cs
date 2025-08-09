using System;
using HireSphere.Core.Enums;

namespace HireSphere.Core.Models;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public Role Role { get; private set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Phone { get; set; }
    public bool IsEmailConfirmed { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public User(Guid id, string email, string passwordHash, Role role, string name, string surname, string phone, bool isEmailConfirmed, DateTime createdAt)
    {
        Id = id;
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
        Name = name;
        Surname = surname;
        Phone = phone;
        IsEmailConfirmed = isEmailConfirmed;
        CreatedAt = createdAt;
    }
}

