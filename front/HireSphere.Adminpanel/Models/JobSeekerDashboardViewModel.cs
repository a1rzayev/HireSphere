using System;

namespace HireSphere.Adminpanel.Models
{
    public class JobSeekerDashboardViewModel
    {
        public List<JobApplicationViewModel> Applications { get; set; } = new();
        public List<JobViewModel> RecommendedJobs { get; set; } = new();
        public int TotalApplications { get; set; }
        public int PendingApplications { get; set; }
        public int ApprovedApplications { get; set; }
        public int RejectedApplications { get; set; }
    }
}
