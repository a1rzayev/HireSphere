using System.ComponentModel.DataAnnotations;

namespace HireSphere.Presentation.Models;

public class CompanyViewModel
{
    public Guid? Id { get; set; }

    [Required(ErrorMessage = "Company name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Company name must be between 2 and 100 characters")]
    [Display(Name = "Company Name")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Company description is required")]
    [StringLength(1000, MinimumLength = 10, ErrorMessage = "Company description must be between 10 and 1000 characters")]
    [Display(Name = "Description")]
    public string Description { get; set; } = string.Empty;

    [Display(Name = "Website")]
    [Url(ErrorMessage = "Please enter a valid URL")]
    public string? Website { get; set; }

    [Display(Name = "Location")]
    [StringLength(100, ErrorMessage = "Location cannot exceed 100 characters")]
    public string? Location { get; set; }

    [Display(Name = "Logo URL")]
    [Url(ErrorMessage = "Please enter a valid URL")]
    public string? LogoUrl { get; set; }

    public Guid? OwnerUserId { get; set; }
    public DateTime? CreatedAt { get; set; }
}

public class CompanyListViewModel
{
    public List<CompanyViewModel> Companies { get; set; } = new();
    public int TotalCount { get; set; }
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public string? LocationFilter { get; set; }
}
