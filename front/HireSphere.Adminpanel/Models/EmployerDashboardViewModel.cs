using System;

namespace HireSphere.Adminpanel.Models
{
    public class EmployerDashboardViewModel
    {
        public List<JobViewModel> Jobs { get; set; } = new();
        public List<JobApplicationViewModel> Applications { get; set; } = new();
        public int TotalJobs { get; set; }
        public int ActiveJobs { get; set; }
        public int TotalApplications { get; set; }
        public int PendingApplications { get; set; }
        public int ApprovedApplications { get; set; }
        public int RejectedApplications { get; set; }
    }
}
