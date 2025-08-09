using System;
using HireSphere.Core.Enums;

namespace HireSphere.Core.Filters;

public class JobSearchFilter
{
    public string? Title { get; set; }
    public Guid? CompanyId { get; set; }
    public Guid? CategoryId { get; set; }
    public JobType? JobType { get; set; }
    public bool? IsRemote { get; set; }
    public decimal? MinSalary { get; set; }
    public decimal? MaxSalary { get; set; }
    public bool? IsActive { get; set; }
}
