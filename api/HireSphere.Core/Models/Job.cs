using System;
using System.Collections.Generic;
using HireSphere.Core.Enums;

namespace HireSphere.Core.Models;

public class Job
{
    public Guid Id { get; set; }
    public Guid CompanyId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Requirements { get; set; }
    public decimal? SalaryFrom { get; set; }
    public decimal? SalaryTo { get; set; }
    public string Location { get; set; }
    public JobType JobType { get; set; }
    public bool IsRemote { get; set; }
    public Guid CategoryId { get; set; }
    public List<string> Tags { get; set; }
    public DateTime PostedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsActive { get; set; }

    public Job(Guid id, Guid companyId, string title, string description, string requirements, decimal? salaryFrom, decimal? salaryTo, string location, JobType jobType, bool isRemote, Guid categoryId, List<string> tags, DateTime postedAt, DateTime expiresAt, bool isActive)
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
        CategoryId = categoryId;
        Tags = tags ?? new List<string>();
        PostedAt = postedAt;
        ExpiresAt = expiresAt;
        IsActive = isActive;
    }
}
