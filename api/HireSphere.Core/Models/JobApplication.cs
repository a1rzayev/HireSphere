using System;
using HireSphere.Core.Enums;

namespace HireSphere.Core.Models;

public class JobApplication
{
    public Guid Id { get; set; }
    public Guid JobId { get; set; }
    public Guid ApplicantUserId { get; set; }
    public string ResumeUrl { get; set; }
    public string CoverLetter { get; set; }
    public JobApplicationStatus Status { get; set; }
    public DateTime AppliedAt { get; set; }

    public JobApplication(Guid id, Guid jobId, Guid applicantUserId, string resumeUrl, string coverLetter, JobApplicationStatus status, DateTime appliedAt)
    {
        Id = id;
        JobId = jobId;
        ApplicantUserId = applicantUserId;
        ResumeUrl = resumeUrl;
        CoverLetter = coverLetter;
        Status = status;
        AppliedAt = appliedAt;
    }
}
