using System.ComponentModel.DataAnnotations;

namespace HireSphere.Adminpanel.Models;

public class UserDetailsViewModel
{
    public Guid Id { get; set; }
    
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;
    
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;
    
    [Display(Name = "Role")]
    public string RoleName { get; set; } = string.Empty;
    
    [Display(Name = "Role")]
    public int RoleValue { get; set; }
    
    [Display(Name = "Phone")]
    public string? Phone { get; set; }
    
    [Display(Name = "User ID")]
    public string UserId { get; set; } = string.Empty;
    
    [Display(Name = "Created")]
    public DateTime? CreatedAt { get; set; }
    
    [Display(Name = "Email Confirmed")]
    public bool? IsEmailConfirmed { get; set; }
    
    public string RoleClass => RoleValue switch
    {
        0 => "badge bg-danger fs-6",
        1 => "badge bg-warning fs-6", 
        2 => "badge bg-info fs-6",
        _ => "badge bg-secondary fs-6"
    };
    
    public string EmailStatusClass => IsEmailConfirmed switch
    {
        true => "badge bg-success",
        false => "badge bg-warning",
        _ => "badge bg-secondary"
    };
    
    public string EmailStatusText => IsEmailConfirmed switch
    {
        true => "Confirmed",
        false => "Not Confirmed", 
        _ => "Unknown"
    };
    
    public string CreatedAtFormatted => CreatedAt?.ToString("MMMM dd, yyyy 'at' HH:mm") ?? "Unknown";
    
    public string PhoneDisplay => !string.IsNullOrEmpty(Phone) ? Phone : "Not provided";
    
    public bool HasPhone => !string.IsNullOrEmpty(Phone);
}
