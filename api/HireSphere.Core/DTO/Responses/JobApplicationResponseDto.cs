namespace HireSphere.Core.DTO.Responses;

public class JobApplicationResponseDto
{
    public Guid Id { get; set; }
    public Guid JobId { get; set; }
    public Guid ApplicantUserId { get; set; }
    public string ResumeUrl { get; set; } = string.Empty;
    public string? CoverLetter { get; set; }
    public int Status { get; set; }
    public DateTime AppliedAt { get; set; }
}
