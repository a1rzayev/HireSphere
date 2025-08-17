using System;

namespace HireSphere.Adminpanel.Models
{
    public class CompanyViewModel
    {
        public Guid Id { get; set; }
        public Guid OwnerUserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Website { get; set; }
        public string? LogoUrl { get; set; }
        public string? Location { get; set; }
        public DateTime CreatedAt { get; set; }
        
        public string TruncatedDescription => Description?.Length > 100 ? Description.Substring(0, 100) + "..." : Description ?? "No description";
        public string DisplayWebsite => !string.IsNullOrEmpty(Website) ? Website : "N/A";
        public string DisplayLocation => !string.IsNullOrEmpty(Location) ? Location : "N/A";
        public bool HasLogo => !string.IsNullOrEmpty(LogoUrl);
    }
}
