using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace HireSphere.Core.Models;

public class Category
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Category name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Category name must be between 2 and 100 characters")]
    [RegularExpression(@"^[a-zA-Z0-9\s\-]+$", ErrorMessage = "Category name can only contain letters, numbers, spaces, and hyphens")]
    public string Name { get; set; } = string.Empty;

    public string Slug { get; private set; } = string.Empty;

    public Category() 
    {
        Name = string.Empty;
        Slug = string.Empty;
    }

    public Category(Guid id, string name)
    {
        Id = id;
        Name = name;
        
        Slug = GenerateSlug(name);

        ValidateName(name);
    }

    private void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Category name must be between 2 and 100 characters.");
        }

        string trimmedName = name.Trim();

        if (trimmedName.Length < 2 || trimmedName.Length > 100)
        {
            throw new ArgumentException("Category name must be between 2 and 100 characters.");
        }

        if (!Regex.IsMatch(trimmedName, @"^[a-zA-Z0-9\s\-]+$"))
        {
            throw new ArgumentException("Category name can only contain letters, numbers, spaces, and hyphens");
        }
    }

    private string GenerateSlug(string name)
    {
        string trimmedName = name.Trim();

        if (trimmedName.Length < 2)
        {
            return "category";
        }

        string slug = Regex.Replace(trimmedName.ToLowerInvariant(), @"[^a-z0-9\s-]", "");
        
        slug = Regex.Replace(slug, @"\s+", "-").Trim('-');

        return string.IsNullOrWhiteSpace(slug) ? "category" : slug;
    }

    public void UpdateName(string newName)
    {
        string newSlug = GenerateSlug(newName);

        ValidateName(newName);

        Name = newName.Trim();
        Slug = newSlug;
    }
}
