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
    public required string Name { get; set; }

    public string Slug { get; private set; }

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

        ValidateCategory();
    }

    private void ValidateCategory()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            throw new ArgumentException("Category name is required.");
        }

        if (Name.Length < 2 || Name.Length > 100)
        {
            throw new ArgumentException("Category name must be between 2 and 100 characters.");
        }
    }

    private string GenerateSlug(string name)
    {
        string slug = Regex.Replace(name.ToLowerInvariant(), @"[^a-z0-9\s-]", "");
        slug = Regex.Replace(slug, @"\s+", "-").Trim('-');

        return string.IsNullOrWhiteSpace(slug) ? "category" : slug;
    }

    public void UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
        {
            throw new ArgumentException("Category name is required.");
        }

        if (newName.Length < 2 || newName.Length > 100)
        {
            throw new ArgumentException("Category name must be between 2 and 100 characters.");
        }

        Name = newName;
        Slug = GenerateSlug(newName);
    }
}
