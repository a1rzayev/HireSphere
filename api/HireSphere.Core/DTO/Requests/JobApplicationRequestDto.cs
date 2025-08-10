namespace HireSphere.Core.DTO.Requests;

public class JobApplicationRequestDto
{
    public Guid JobId { get; set; }
    public Guid ApplicantUserId { get; set; }
    public string ResumeUrl { get; set; } = string.Empty;
    public string? CoverLetter { get; set; }
}
