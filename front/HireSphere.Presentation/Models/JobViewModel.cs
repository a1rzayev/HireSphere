using System;

namespace HireSphere.Presentation.Models;

public class JobViewModel
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Requirements { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public decimal SalaryFrom { get; set; }
    public decimal SalaryTo { get; set; }
    public string JobType { get; set; } = string.Empty;
    public bool IsRemote { get; set; }
    public DateTime PostedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new List<string>();

    public string SalaryRange => SalaryFrom > 0 && SalaryTo > 0 
        ? $"${SalaryFrom:N0} - ${SalaryTo:N0}" 
        : SalaryFrom > 0 
            ? $"${SalaryFrom:N0}+" 
            : "Salary not specified";

    public string PostedDate => PostedAt.ToString("MMM dd, yyyy");
    public string ExpiresDate => ExpiresAt.ToString("MMM dd, yyyy");
    public bool IsExpired => DateTime.Now > ExpiresAt;
    public int DaysSincePosted => (DateTime.Now - PostedAt).Days;
    public int DaysUntilExpires => (ExpiresAt - DateTime.Now).Days;
}

public class JobsListViewModel
{
    public List<JobViewModel> Jobs { get; set; } = new();
    public string SearchTerm { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string JobType { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public bool IsRemote { get; set; }
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalPages { get; set; } = 1;
    public int TotalCount { get; set; } = 0;

    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;
    public int PreviousPage => Math.Max(1, CurrentPage - 1);
    public int NextPage => Math.Min(TotalPages, CurrentPage + 1);
}
