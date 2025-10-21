using System.ComponentModel.DataAnnotations;

namespace HireSphere.Adminpanel.Models;

public class CategoryViewModel
{
    public Guid Id { get; set; }
    
    [Required(ErrorMessage = "Category name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Category name must be between 2 and 100 characters")]
    public string Name { get; set; } = string.Empty;
    
    public string Slug { get; set; } = string.Empty;
    
    public int JobCount { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public string FormattedCreatedAt => CreatedAt.ToString("MMM dd, yyyy");
}
