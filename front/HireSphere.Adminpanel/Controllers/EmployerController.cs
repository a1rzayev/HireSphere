using Microsoft.AspNetCore.Mvc;
using HireSphere.Adminpanel.Models;
using HireSphere.Adminpanel.Filters;
using System.Text;
using System.Text.Json;

namespace HireSphere.Adminpanel.Controllers;

[ServiceFilter(typeof(EmployerOnlyFilter))]
public class EmployerController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private readonly ILogger<EmployerController> _logger;

    public EmployerController(IConfiguration configuration, HttpClient httpClient, ILogger<EmployerController> logger)
    {
        _configuration = configuration;
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<IActionResult> Dashboard()
    {
        try
        {
            var baseUrl = _configuration["BASE_URL"];
            var accessToken = HttpContext.Session.GetString("AccessToken");

            if (string.IsNullOrEmpty(accessToken))
            {
                return RedirectToAction("Login", "Auth");
            }

            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            // Get employer's jobs
            var jobsResponse = await _httpClient.GetAsync($"{baseUrl}/api/jobs/my-jobs");
            var jobs = new List<JobViewModel>();

            if (jobsResponse.IsSuccessStatusCode)
            {
                var jobsContent = await jobsResponse.Content.ReadAsStringAsync();
                var jobsData = JsonSerializer.Deserialize<JsonElement>(jobsContent);
                
                if (jobsData.TryGetProperty("data", out var jobsArray))
                {
                    foreach (var job in jobsArray.EnumerateArray())
                    {
                        jobs.Add(new JobViewModel
                        {
                            Id = job.TryGetProperty("id", out var id) ? Guid.Parse(id.GetString() ?? "") : Guid.Empty,
                            Title = job.TryGetProperty("title", out var title) ? title.GetString() ?? "" : "",
                            Description = job.TryGetProperty("description", out var desc) ? desc.GetString() ?? "" : "",
                            Location = job.TryGetProperty("location", out var loc) ? loc.GetString() ?? "" : "",
                            SalaryFrom = job.TryGetProperty("salaryFrom", out var salFrom) ? salFrom.GetDecimal() : 0,
                            SalaryTo = job.TryGetProperty("salaryTo", out var salTo) ? salTo.GetDecimal() : 0,
                            JobType = job.TryGetProperty("jobType", out var type) ? type.GetString() ?? "" : "",
                            IsRemote = job.TryGetProperty("isRemote", out var remote) ? remote.GetBoolean() : false,
                            IsActive = job.TryGetProperty("isActive", out var active) ? active.GetBoolean() : false,
                            PostedAt = job.TryGetProperty("postedAt", out var posted) ? DateTime.Parse(posted.GetString() ?? "") : DateTime.Now,
                            ExpiresAt = job.TryGetProperty("expiresAt", out var expires) ? DateTime.Parse(expires.GetString() ?? "") : DateTime.Now.AddDays(30)
                        });
                    }
                }
            }

            // Get job applications for employer's jobs
            var applicationsResponse = await _httpClient.GetAsync($"{baseUrl}/api/job-applications/employer");
            var applications = new List<JobApplicationViewModel>();

            if (applicationsResponse.IsSuccessStatusCode)
            {
                var applicationsContent = await applicationsResponse.Content.ReadAsStringAsync();
                var applicationsData = JsonSerializer.Deserialize<JsonElement>(applicationsContent);
                
                if (applicationsData.TryGetProperty("data", out var applicationsArray))
                {
                    foreach (var app in applicationsArray.EnumerateArray())
                    {
                        applications.Add(new JobApplicationViewModel
                        {
                            Id = app.TryGetProperty("id", out var id) ? Guid.Parse(id.GetString() ?? "") : Guid.Empty,
                            JobTitle = app.TryGetProperty("jobTitle", out var jobTitle) ? jobTitle.GetString() ?? "" : "",
                            ApplicantName = app.TryGetProperty("applicantName", out var applicant) ? applicant.GetString() ?? "" : "",
                            Status = app.TryGetProperty("status", out var status) ? status.GetString() ?? "" : "",
                            AppliedAt = app.TryGetProperty("appliedAt", out var applied) ? DateTime.Parse(applied.GetString() ?? "") : DateTime.Now
                        });
                    }
                }
            }

            var dashboardData = new EmployerDashboardViewModel
            {
                Jobs = jobs,
                Applications = applications,
                TotalJobs = jobs.Count,
                ActiveJobs = jobs.Count(j => j.IsActive),
                TotalApplications = applications.Count,
                PendingApplications = applications.Count(a => a.Status == "Pending")
            };

            return View(dashboardData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading employer dashboard");
            return View(new EmployerDashboardViewModel());
        }
    }

    public async Task<IActionResult> MyJobs()
    {
        try
        {
            var baseUrl = _configuration["BASE_URL"];
            var accessToken = HttpContext.Session.GetString("AccessToken");

            if (string.IsNullOrEmpty(accessToken))
            {
                return RedirectToAction("Login", "Auth");
            }

            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.GetAsync($"{baseUrl}/api/jobs/my-jobs");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<JsonElement>(content);
                
                var jobs = new List<JobViewModel>();
                if (data.TryGetProperty("data", out var jobsArray))
                {
                    foreach (var job in jobsArray.EnumerateArray())
                    {
                        jobs.Add(new JobViewModel
                        {
                            Id = job.TryGetProperty("id", out var id) ? Guid.Parse(id.GetString() ?? "") : Guid.Empty,
                            Title = job.TryGetProperty("title", out var title) ? title.GetString() ?? "" : "",
                            Description = job.TryGetProperty("description", out var desc) ? desc.GetString() ?? "" : "",
                            Location = job.TryGetProperty("location", out var loc) ? loc.GetString() ?? "" : "",
                            SalaryFrom = job.TryGetProperty("salaryFrom", out var salFrom) ? salFrom.GetDecimal() : 0,
                            SalaryTo = job.TryGetProperty("salaryTo", out var salTo) ? salTo.GetDecimal() : 0,
                            JobType = job.TryGetProperty("jobType", out var type) ? type.GetString() ?? "" : "",
                            IsRemote = job.TryGetProperty("isRemote", out var remote) ? remote.GetBoolean() : false,
                            IsActive = job.TryGetProperty("isActive", out var active) ? active.GetBoolean() : false,
                            PostedAt = job.TryGetProperty("postedAt", out var posted) ? DateTime.Parse(posted.GetString() ?? "") : DateTime.Now,
                            ExpiresAt = job.TryGetProperty("expiresAt", out var expires) ? DateTime.Parse(expires.GetString() ?? "") : DateTime.Now.AddDays(30)
                        });
                    }
                }

                return View(jobs);
            }

            return View(new List<JobViewModel>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading employer jobs");
            return View(new List<JobViewModel>());
        }
    }

    public async Task<IActionResult> Applications()
    {
        try
        {
            var baseUrl = _configuration["BASE_URL"];
            var accessToken = HttpContext.Session.GetString("AccessToken");

            if (string.IsNullOrEmpty(accessToken))
            {
                return RedirectToAction("Login", "Auth");
            }

            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.GetAsync($"{baseUrl}/api/job-applications/employer");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<JsonElement>(content);
                
                var applications = new List<JobApplicationViewModel>();
                if (data.TryGetProperty("data", out var applicationsArray))
                {
                    foreach (var app in applicationsArray.EnumerateArray())
                    {
                        applications.Add(new JobApplicationViewModel
                        {
                            Id = app.TryGetProperty("id", out var id) ? Guid.Parse(id.GetString() ?? "") : Guid.Empty,
                            JobTitle = app.TryGetProperty("jobTitle", out var jobTitle) ? jobTitle.GetString() ?? "" : "",
                            ApplicantName = app.TryGetProperty("applicantName", out var applicant) ? applicant.GetString() ?? "" : "",
                            Status = app.TryGetProperty("status", out var status) ? status.GetString() ?? "" : "",
                            AppliedAt = app.TryGetProperty("appliedAt", out var applied) ? DateTime.Parse(applied.GetString() ?? "") : DateTime.Now
                        });
                    }
                }

                return View(applications);
            }

            return View(new List<JobApplicationViewModel>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading employer applications");
            return View(new List<JobApplicationViewModel>());
        }
    }
}
