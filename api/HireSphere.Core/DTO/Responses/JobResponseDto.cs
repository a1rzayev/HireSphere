namespace HireSphere.Core.DTO.Responses;

public class JobResponseDto
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Requirements { get; set; }
    public decimal? SalaryFrom { get; set; }
    public decimal? SalaryTo { get; set; }
    public string? Location { get; set; }
    public int JobType { get; set; }
    public bool IsRemote { get; set; }
    public Guid CategoryId { get; set; }
    public List<string>? Tags { get; set; }
    public DateTime PostedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsActive { get; set; }
}
