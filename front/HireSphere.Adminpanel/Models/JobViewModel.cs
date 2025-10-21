using System.ComponentModel.DataAnnotations;

namespace HireSphere.Adminpanel.Models;

public class JobViewModel
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Job title is required")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "Job title must be between 2 and 200 characters")]
    public string Title { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Job description is required")]
    [StringLength(2000, MinimumLength = 10, ErrorMessage = "Job description must be between 10 and 2000 characters")]
    public string Description { get; set; } = string.Empty;
    
    public string? Requirements { get; set; }
    
    [Range(0, double.MaxValue, ErrorMessage = "Salary from must be a non-negative value")]
    public decimal? SalaryFrom { get; set; }
    
    [Range(0, double.MaxValue, ErrorMessage = "Salary to must be a non-negative value")]
    public decimal? SalaryTo { get; set; }
    
    public string? Location { get; set; }
    
    [Required(ErrorMessage = "Job type is required")]
    public string JobType { get; set; } = string.Empty;
    
    public bool IsRemote { get; set; }
    
    [Required(ErrorMessage = "Category ID is required")]
    public Guid CategoryId { get; set; }
    
    public string CategoryName { get; set; } = string.Empty;
    
    public List<string> Tags { get; set; } = new List<string>();
    
    public DateTime PostedAt { get; set; }
    
    public DateTime ExpiresAt { get; set; }
    
    public bool IsActive { get; set; }
    
    public int ApplicationCount { get; set; }
    
    public string Status => IsActive ? "Active" : "Inactive";
    
    public string SalaryRange
    {
        get
        {
            if (SalaryFrom.HasValue && SalaryTo.HasValue)
                return $"{SalaryFrom:C} - {SalaryTo:C}";
            else if (SalaryFrom.HasValue)
                return $"From {SalaryFrom:C}";
            else if (SalaryTo.HasValue)
                return $"Up to {SalaryTo:C}";
            else
                return "Not specified";
        }
    }
}

public class JobIndexViewModel
{
    public List<JobViewModel> Jobs { get; set; } = new List<JobViewModel>();
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalPages { get; set; }
    public string SearchTerm { get; set; } = string.Empty;
    public string StatusFilter { get; set; } = string.Empty;
    public List<string> JobTypes { get; set; } = new List<string> { "FullTime", "PartTime", "Contract", "Internship", "Temporary" };
    public List<string> StatusOptions { get; set; } = new List<string> { "Active", "Inactive" };
}
