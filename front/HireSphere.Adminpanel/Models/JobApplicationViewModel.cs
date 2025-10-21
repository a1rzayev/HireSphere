using System.ComponentModel.DataAnnotations;

namespace HireSphere.Adminpanel.Models;

public class JobApplicationViewModel
{
    public Guid Id { get; set; }
    public Guid JobId { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public Guid ApplicantUserId { get; set; }
    public string ApplicantName { get; set; } = string.Empty;
    public string ApplicantEmail { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Resume URL is required")]
    [Url(ErrorMessage = "Invalid resume URL format")]
    public string ResumeUrl { get; set; } = string.Empty;
    
    public string? CoverLetter { get; set; }
    
    [Required(ErrorMessage = "Application status is required")]
    public string Status { get; set; } = string.Empty;
    
    public DateTime AppliedAt { get; set; }
    
    public string StatusColor
    {
        get
        {
            return Status switch
            {
                "Applied" => "primary",
                "Screening" => "info",
                "Interview" => "warning",
                "Offered" => "success",
                "Accepted" => "success",
                "Rejected" => "danger",
                "Withdrawn" => "secondary",
                _ => "secondary"
            };
        }
    }
    
    public string FormattedAppliedAt => AppliedAt.ToString("MMM dd, yyyy HH:mm");
}

public class JobApplicationIndexViewModel
{
    public List<JobApplicationViewModel> Applications { get; set; } = new List<JobApplicationViewModel>();
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalPages { get; set; }
    public string SearchTerm { get; set; } = string.Empty;
    public string StatusFilter { get; set; } = string.Empty;
    public List<string> StatusOptions { get; set; } = new List<string> 
    { 
        "Applied", "Screening", "Interview", "Offered", "Accepted", "Rejected", "Withdrawn" 
    };
}

public class JobApplicationStatisticsViewModel
{
    public int TotalApplications { get; set; }
    public int AppliedCount { get; set; }
    public int ScreeningCount { get; set; }
    public int InterviewCount { get; set; }
    public int OfferedCount { get; set; }
    public int AcceptedCount { get; set; }
    public int RejectedCount { get; set; }
    public int WithdrawnCount { get; set; }
    
    public List<ApplicationStatusCount> StatusCounts { get; set; } = new List<ApplicationStatusCount>();
    public List<MonthlyApplicationCount> MonthlyApplications { get; set; } = new List<MonthlyApplicationCount>();
    public List<TopJobApplications> TopJobs { get; set; } = new List<TopJobApplications>();
}

public class ApplicationStatusCount
{
    public string Status { get; set; } = string.Empty;
    public int Count { get; set; }
    public double Percentage { get; set; }
}

public class MonthlyApplicationCount
{
    public string Month { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class TopJobApplications
{
    public Guid JobId { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public int ApplicationCount { get; set; }
}
