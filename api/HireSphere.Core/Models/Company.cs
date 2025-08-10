using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace HireSphere.Core.Models;

public class Company
{
    public Guid Id { get; set; }
    public Guid OwnerUserId { get; set; }

    [Required(ErrorMessage = "Company name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Company name must be between 2 and 100 characters")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; set; }

    [RegularExpression(@"^(https?://)?([a-zA-Z0-9-]+\.)+[a-zA-Z]{2,}(/.*)?$", ErrorMessage = "Invalid website URL format")]
    public string? Website { get; set; }

    public string? LogoUrl { get; set; }
    public string? Location { get; set; }
    public DateTime CreatedAt { get; set; }

    public Company() 
    {
        CreatedAt = DateTime.UtcNow;
    }

    public Company(Guid id, Guid ownerUserId, string name, string? description = null, string? website = null, string? logoUrl = null, string? location = null, DateTime? createdAt = null)
    {
        Id = id;
        OwnerUserId = ownerUserId;
        Name = name;
        Description = description;
        Website = website;
        LogoUrl = logoUrl;
        Location = location;
        CreatedAt = createdAt ?? DateTime.UtcNow;

        ValidateCompany();
    }

    public void ValidateCompany()
    {
        if (string.IsNullOrWhiteSpace(Name) || Name.Trim().Length < 2 || Name.Trim().Length > 100)
        {
            throw new ArgumentException("Company name must be between 2 and 100 characters");
        }

        if (Description != null && Description.Length > 500)
        {
            throw new ArgumentException("Description cannot exceed 500 characters");
        }

        if (!string.IsNullOrWhiteSpace(Website))
        {
            if (!Website.StartsWith("http://") && !Website.StartsWith("https://"))
            {
                Website = $"http://{Website}";
            }

            if (!Regex.IsMatch(Website, @"^(https?://)?([a-zA-Z0-9-]+\.)+[a-zA-Z]{2,}(/.*)?$"))
            {
                throw new ArgumentException("Invalid website URL format");
            }
        }
    }

    public void UpdateLogoUrl(string? newLogoUrl)
    {
        if (!string.IsNullOrWhiteSpace(newLogoUrl) && 
            !Uri.TryCreate(newLogoUrl, UriKind.Absolute, out _))
        {
            throw new ArgumentException("Invalid logo URL format");
        }
        LogoUrl = newLogoUrl;
    }
}
