using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using HireSphere.Core.Enums;

namespace HireSphere.Core.Models;

public class Job
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Company ID is required")]
    public Guid CompanyId { get; set; }

    [Required(ErrorMessage = "Job title is required")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "Job title must be between 2 and 200 characters")]
    public required string Title { get; set; }

    [Required(ErrorMessage = "Job description is required")]
    [StringLength(2000, MinimumLength = 10, ErrorMessage = "Job description must be between 10 and 2000 characters")]
    public required string Description { get; set; }

    public string? Requirements { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Salary from must be a non-negative value")]
    public decimal? SalaryFrom { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Salary to must be a non-negative value")]
    public decimal? SalaryTo { get; set; }

    public string? Location { get; set; }

    [Required(ErrorMessage = "Job type is required")]
    public JobType JobType { get; set; }

    public bool IsRemote { get; set; }

    [Required(ErrorMessage = "Category ID is required")]
    public Guid CategoryId { get; set; }

    public List<string> Tags { get; set; } = new List<string>();

    public DateTime PostedAt { get; set; }

    public DateTime ExpiresAt { get; set; }

    public bool IsActive { get; set; }

    public Job() 
    {
        Title = string.Empty;
        Description = string.Empty;
        PostedAt = DateTime.UtcNow;
        ExpiresAt = PostedAt.AddDays(30); 
        IsActive = true;
    }

    public Job(Guid id, Guid companyId, string title, string description, string? requirements = null, 
               decimal? salaryFrom = null, decimal? salaryTo = null, string? location = null, 
               JobType jobType = JobType.FullTime, bool isRemote = false, 
               Guid? categoryId = null, List<string>? tags = null, 
               DateTime? postedAt = null, DateTime? expiresAt = null, bool isActive = true)
    {
        Id = id;
        CompanyId = companyId;
        Title = title;
        Description = description;
        Requirements = requirements;
        SalaryFrom = salaryFrom;
        SalaryTo = salaryTo;
        Location = location;
        JobType = jobType;
        IsRemote = isRemote;
        CategoryId = categoryId ?? Guid.Empty;
        Tags = tags ?? new List<string>();
        PostedAt = postedAt ?? DateTime.UtcNow;
        ExpiresAt = expiresAt ?? PostedAt.AddDays(30);
        IsActive = isActive;

        ValidateJob();
    }

    private void ValidateJob()
    {
        if (SalaryFrom.HasValue && SalaryTo.HasValue && SalaryFrom > SalaryTo)
        {
            throw new ArgumentException("Salary 'from' cannot be greater than salary 'to'.");
        }

        if (ExpiresAt < PostedAt)
        {
            throw new ArgumentException("Job expiration date must be after the posting date.");
        }

        if (string.IsNullOrWhiteSpace(Title))
        {
            throw new ArgumentException("Job title is required.");
        }

        if (string.IsNullOrWhiteSpace(Description))
        {
            throw new ArgumentException("Job description is required.");
        }

        if (!Enum.IsDefined(typeof(JobType), JobType))
        {
            throw new ArgumentException("Invalid job type.");
        }
    }

    public void Activate()
    {
        if (IsExpired())
        {
            throw new InvalidOperationException("Cannot activate an expired job.");
        }
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public bool IsExpired()
    {
        return DateTime.UtcNow > ExpiresAt;
    }

    public void ExtendExpiration(int days)
    {
        if (days <= 0)
        {
            throw new ArgumentException("Extension days must be positive.");
        }
        ExpiresAt = ExpiresAt.AddDays(days);
    }
}
