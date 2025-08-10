using System.ComponentModel.DataAnnotations;

namespace HireSphere.Core.DTO.Requests;

public class ForgotPasswordRequestDto
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = string.Empty;
}
