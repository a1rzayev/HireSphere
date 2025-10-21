namespace HireSphere.Adminpanel.Models;

public class DashboardViewModel
{
    public UserStatisticsViewModel UserStatistics { get; set; } = new UserStatisticsViewModel();
    public JobStatisticsViewModel JobStatistics { get; set; } = new JobStatisticsViewModel();
    public CompanyStatisticsViewModel CompanyStatistics { get; set; } = new CompanyStatisticsViewModel();
    public JobApplicationStatisticsViewModel ApplicationStatistics { get; set; } = new JobApplicationStatisticsViewModel();
}

public class UserStatisticsViewModel
{
    public int TotalUsers { get; set; }
    public int AdminCount { get; set; }
    public int EmployerCount { get; set; }
    public int JobSeekerCount { get; set; }
    public int ActiveUsers { get; set; }
    public int InactiveUsers { get; set; }
    public int NewUsersThisMonth { get; set; }
    public int NewUsersThisWeek { get; set; }
    
    public List<MonthlyUserCount> MonthlyUsers { get; set; } = new List<MonthlyUserCount>();
    public List<UserRoleCount> RoleCounts { get; set; } = new List<UserRoleCount>();
}

public class JobStatisticsViewModel
{
    public int TotalJobs { get; set; }
    public int ActiveJobs { get; set; }
    public int InactiveJobs { get; set; }
    public int ExpiredJobs { get; set; }
    public int RemoteJobs { get; set; }
    public int OnSiteJobs { get; set; }
    public int NewJobsThisMonth { get; set; }
    public int NewJobsThisWeek { get; set; }
    
    public List<MonthlyJobCount> MonthlyJobs { get; set; } = new List<MonthlyJobCount>();
    public List<JobTypeCount> JobTypeCounts { get; set; } = new List<JobTypeCount>();
    public List<TopCompanyJobs> TopCompanies { get; set; } = new List<TopCompanyJobs>();
}

public class CompanyStatisticsViewModel
{
    public int TotalCompanies { get; set; }
    public int VerifiedCompanies { get; set; }
    public int UnverifiedCompanies { get; set; }
    public int NewCompaniesThisMonth { get; set; }
    public int NewCompaniesThisWeek { get; set; }
    
    public List<MonthlyCompanyCount> MonthlyCompanies { get; set; } = new List<MonthlyCompanyCount>();
    public List<CompanyLocationCount> LocationCounts { get; set; } = new List<CompanyLocationCount>();
}

public class MonthlyUserCount
{
    public string Month { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class MonthlyJobCount
{
    public string Month { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class MonthlyCompanyCount
{
    public string Month { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class UserRoleCount
{
    public string Role { get; set; } = string.Empty;
    public int Count { get; set; }
    public double Percentage { get; set; }
}

public class JobTypeCount
{
    public string JobType { get; set; } = string.Empty;
    public int Count { get; set; }
    public double Percentage { get; set; }
}

public class CompanyLocationCount
{
    public string Location { get; set; } = string.Empty;
    public int Count { get; set; }
    public double Percentage { get; set; }
}

public class TopCompanyJobs
{
    public Guid CompanyId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public int JobCount { get; set; }
}

public class UserAnalyticsViewModel
{
    public UserStatisticsViewModel Statistics { get; set; } = new UserStatisticsViewModel();
    public List<RecentUser> RecentUsers { get; set; } = new List<RecentUser>();
    public List<UserActivity> UserActivities { get; set; } = new List<UserActivity>();
}

public class JobAnalyticsViewModel
{
    public JobStatisticsViewModel Statistics { get; set; } = new JobStatisticsViewModel();
    public List<RecentJob> RecentJobs { get; set; } = new List<RecentJob>();
    public List<JobActivity> JobActivities { get; set; } = new List<JobActivity>();
}

public class CompanyAnalyticsViewModel
{
    public CompanyStatisticsViewModel Statistics { get; set; } = new CompanyStatisticsViewModel();
    public List<RecentCompany> RecentCompanies { get; set; } = new List<RecentCompany>();
    public List<CompanyActivity> CompanyActivities { get; set; } = new List<CompanyActivity>();
}

public class RecentUser
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class RecentJob
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string JobType { get; set; } = string.Empty;
    public DateTime PostedAt { get; set; }
}

public class RecentCompany
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class UserActivity
{
    public string Activity { get; set; } = string.Empty;
    public int Count { get; set; }
    public DateTime Date { get; set; }
}

public class JobActivity
{
    public string Activity { get; set; } = string.Empty;
    public int Count { get; set; }
    public DateTime Date { get; set; }
}

public class CompanyActivity
{
    public string Activity { get; set; } = string.Empty;
    public int Count { get; set; }
    public DateTime Date { get; set; }
}
