using System;

namespace HireSphere.Adminpanel.Models
{
    public class UserViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public int Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsEmailConfirmed { get; set; }
        
        public string FullName => $"{Name} {Surname}".Trim();
        public string RoleName => Role switch
        {
            0 => "Admin",
            1 => "Employer", 
            2 => "Job Seeker",
            _ => "Unknown"
        };
        
        public string RoleClass => Role switch
        {
            0 => "badge bg-danger",
            1 => "badge bg-warning",
            2 => "badge bg-info",
            _ => "badge bg-secondary"
        };
    }
}
