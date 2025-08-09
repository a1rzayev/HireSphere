using System;
using System.ComponentModel.DataAnnotations;
using HireSphere.Core.Enums;

namespace HireSphere.Core.Models;

public class JobApplication
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Job ID is required")]
    public Guid JobId { get; set; }

    [Required(ErrorMessage = "Applicant User ID is required")]
    public Guid ApplicantUserId { get; set; }

    [Required(ErrorMessage = "Resume URL is required")]
    [Url(ErrorMessage = "Invalid resume URL format")]
    public required string ResumeUrl { get; set; }

    public string? CoverLetter { get; set; }

    [Required(ErrorMessage = "Application status is required")]
    public JobApplicationStatus Status { get; set; }

    public DateTime AppliedAt { get; set; }

    public JobApplication() 
    {
        ResumeUrl = string.Empty;
        Status = JobApplicationStatus.Applied;
        AppliedAt = DateTime.UtcNow;
    }

    public JobApplication(Guid id, Guid jobId, Guid applicantUserId, string resumeUrl, 
                          string? coverLetter = null, 
                          JobApplicationStatus status = JobApplicationStatus.Applied, 
                          DateTime? appliedAt = null)
    {
        Id = id;
        JobId = jobId;
        ApplicantUserId = applicantUserId;
        ResumeUrl = resumeUrl;
        CoverLetter = coverLetter;
        Status = status;
        AppliedAt = appliedAt ?? DateTime.UtcNow;

        ValidateApplication();
    }

    private void ValidateApplication()
    {
        if (!Uri.TryCreate(ResumeUrl, UriKind.Absolute, out _))
        {
            throw new ArgumentException("Invalid resume URL format.");
        }

        if (!Enum.IsDefined(typeof(JobApplicationStatus), Status))
        {
            throw new ArgumentException("Invalid application status.");
        }
    }


    // Domain method to change application status with business rules
    public void ChangeStatus(JobApplicationStatus newStatus)
    {
        // Business rules for status transitions
        switch (Status)
        {
            case JobApplicationStatus.Applied:
                // From Applied, can move to: Screening, Rejected
                if (newStatus != JobApplicationStatus.Screening && 
                    newStatus != JobApplicationStatus.Rejected)
                {
                    throw new InvalidOperationException("Invalid status transition from Applied.");
                }
                break;

            case JobApplicationStatus.Screening:
                // From Screening, can move to: Interview, Rejected
                if (newStatus != JobApplicationStatus.Interview && 
                    newStatus != JobApplicationStatus.Rejected)
                {
                    throw new InvalidOperationException("Invalid status transition from Screening.");
                }
                break;

            case JobApplicationStatus.Interview:
                // From Interview, can move to: Offered, Rejected
                if (newStatus != JobApplicationStatus.Offered && 
                    newStatus != JobApplicationStatus.Rejected)
                {
                    throw new InvalidOperationException("Invalid status transition from Interview.");
                }
                break;

            case JobApplicationStatus.Offered:
                // From Offered, can move to: Accepted, Rejected
                if (newStatus != JobApplicationStatus.Accepted && 
                    newStatus != JobApplicationStatus.Rejected)
                {
                    throw new InvalidOperationException("Invalid status transition from Offered.");
                }
                break;

            case JobApplicationStatus.Rejected:
                // Once Rejected, no further transitions allowed
                throw new InvalidOperationException("Cannot change status from Rejected.");

            case JobApplicationStatus.Accepted:
                // Once Accepted, no further transitions allowed
                throw new InvalidOperationException("Cannot change status from Accepted.");

            case JobApplicationStatus.Withdrawn:
                // Once Withdrawn, no further transitions allowed
                throw new InvalidOperationException("Cannot change status from Withdrawn.");

            default:
                throw new ArgumentException("Invalid application status.");
        }

        // If all validations pass, update the status
        Status = newStatus;
    }

    public void AddCoverLetter(string coverLetterText)
    {
        if (string.IsNullOrWhiteSpace(coverLetterText))
        {
            throw new ArgumentException("Cover letter cannot be empty.");
        }

        if (coverLetterText.Length > 2000)
        {
            throw new ArgumentException("Cover letter cannot exceed 2000 characters.");
        }

        CoverLetter = coverLetterText;
    }
}
