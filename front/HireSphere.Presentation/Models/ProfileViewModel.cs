using System.ComponentModel.DataAnnotations;

namespace HireSphere.Presentation.Models;

public class ProfileViewModel
{
    [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
    [Display(Name = "First Name")]
    public string? Name { get; set; }

    [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
    [Display(Name = "Last Name")]
    public string? Surname { get; set; }

    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    [Display(Name = "Email")]
    public string? Email { get; set; }

    [Display(Name = "Phone Number")]
    [Phone(ErrorMessage = "Please enter a valid phone number")]
    public string? PhoneNumber { get; set; }

    [Display(Name = "Bio")]
    [StringLength(500, ErrorMessage = "Bio cannot exceed 500 characters")]
    public string? Bio { get; set; }

    [Display(Name = "Location")]
    [StringLength(100, ErrorMessage = "Location cannot exceed 100 characters")]
    public string? Location { get; set; }

    [Display(Name = "Website")]
    [Url(ErrorMessage = "Please enter a valid URL")]
    public string? Website { get; set; }



    [Display(Name = "Company")]
    [StringLength(100, ErrorMessage = "Company cannot exceed 100 characters")]
    public string? Company { get; set; }
}
