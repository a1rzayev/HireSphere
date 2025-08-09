using System;
using HireSphere.Core.Enums;

namespace HireSphere.Core.Filters;

public class JobApplicationSearchFilter
{
    public Guid? JobId { get; set; }
    public Guid? ApplicantUserId { get; set; }
    public JobApplicationStatus? Status { get; set; }
    public DateTime? AppliedAfter { get; set; }
    public DateTime? AppliedBefore { get; set; }
}
