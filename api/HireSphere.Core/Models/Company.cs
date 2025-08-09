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
    public required string Name { get; set; }

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; set; }

    [RegularExpression(@"^(https?://)?([a-zA-Z0-9-]+\.)+[a-zA-Z]{2,}(/.*)?$", ErrorMessage = "Invalid website URL format")]
    public string? Website { get; set; }

    public string? LogoUrl { get; set; }
    public string? Location { get; set; }
    public DateTime CreatedAt { get; set; }

    // Parameterless constructor for ORM
    public Company() 
    {
        // Initialize required properties with default values
        Name = string.Empty;
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
    }

    // Method to validate logo URL format
    public void UpdateLogoUrl(string? newLogoUrl)
    {
        // Optional: Add logo URL validation logic here
        if (!string.IsNullOrWhiteSpace(newLogoUrl) && 
            !Uri.TryCreate(newLogoUrl, UriKind.Absolute, out _))
        {
            throw new ArgumentException("Invalid logo URL format");
        }
        LogoUrl = newLogoUrl;
    }
}
