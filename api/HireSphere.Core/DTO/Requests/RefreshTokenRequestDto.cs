using System.ComponentModel.DataAnnotations;

namespace HireSphere.Core.DTO.Requests;

public class RefreshTokenRequestDto
{
    [Required(ErrorMessage = "Access token is required")]
    public string AccessToken { get; set; } = string.Empty;

    [Required(ErrorMessage = "Refresh token is required")]
    public string RefreshToken { get; set; } = string.Empty;
}
