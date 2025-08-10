namespace HireSphere.Core.DTO.Requests;

public class CompanyRequestDto
{
    public Guid OwnerUserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Website { get; set; }
    public string? LogoUrl { get; set; }
    public string? Location { get; set; }
}
