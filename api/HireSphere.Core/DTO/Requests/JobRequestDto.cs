namespace HireSphere.Core.DTO.Requests;

public class JobRequestDto
{
    public Guid CompanyId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? Requirements { get; set; }
    public decimal? SalaryFrom { get; set; }
    public decimal? SalaryTo { get; set; }
    public string? Location { get; set; }
    public int JobType { get; set; } // int for enum mapping
    public bool IsRemote { get; set; }
    public Guid CategoryId { get; set; }
    public List<string>? Tags { get; set; }
    public DateTime? ExpiresAt { get; set; }
}
