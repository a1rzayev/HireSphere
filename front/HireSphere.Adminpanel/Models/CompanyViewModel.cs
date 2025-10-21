using System;
using System.ComponentModel.DataAnnotations;

namespace HireSphere.Adminpanel.Models
{
    public class CompanyViewModel
    {
        public Guid Id { get; set; }
        public Guid OwnerUserId { get; set; }
        
        [Required(ErrorMessage = "Company name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Company name must be between 2 and 100 characters")]
        [Display(Name = "Company Name")]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        [Display(Name = "Description")]
        public string? Description { get; set; }
        
        [Url(ErrorMessage = "Please enter a valid URL")]
        [Display(Name = "Website")]
        public string? Website { get; set; }
        
        [Url(ErrorMessage = "Please enter a valid URL")]
        [Display(Name = "Logo URL")]
        public string? LogoUrl { get; set; }
        
        [StringLength(100, ErrorMessage = "Location cannot exceed 100 characters")]
        [Display(Name = "Location")]
        public string? Location { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public string TruncatedDescription => Description?.Length > 100 ? Description.Substring(0, 100) + "..." : Description ?? "No description";
        public string DisplayWebsite => !string.IsNullOrEmpty(Website) ? Website : "N/A";
        public string DisplayLocation => !string.IsNullOrEmpty(Location) ? Location : "N/A";
        public bool HasLogo => !string.IsNullOrEmpty(LogoUrl);
    }
}
