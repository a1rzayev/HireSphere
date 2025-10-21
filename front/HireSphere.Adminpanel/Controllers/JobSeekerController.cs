using Microsoft.AspNetCore.Mvc;
using HireSphere.Adminpanel.Models;
using HireSphere.Adminpanel.Filters;
using System.Text;
using System.Text.Json;

namespace HireSphere.Adminpanel.Controllers;

[ServiceFilter(typeof(JobSeekerOnlyFilter))]
public class JobSeekerController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private readonly ILogger<JobSeekerController> _logger;

    public JobSeekerController(IConfiguration configuration, HttpClient httpClient, ILogger<JobSeekerController> logger)
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

            // Get job seeker's applications
            var applicationsResponse = await _httpClient.GetAsync($"{baseUrl}/api/job-applications/my-applications");
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
                            CompanyName = app.TryGetProperty("companyName", out var company) ? company.GetString() ?? "" : "",
                            Status = app.TryGetProperty("status", out var status) ? status.GetString() ?? "" : "",
                            AppliedAt = app.TryGetProperty("appliedAt", out var applied) ? DateTime.Parse(applied.GetString() ?? "") : DateTime.Now
                        });
                    }
                }
            }

            // Get recommended jobs
            var jobsResponse = await _httpClient.GetAsync($"{baseUrl}/api/jobs");
            var recommendedJobs = new List<JobViewModel>();

            if (jobsResponse.IsSuccessStatusCode)
            {
                var jobsContent = await jobsResponse.Content.ReadAsStringAsync();
                var jobsData = JsonSerializer.Deserialize<JsonElement>(jobsContent);
                
                if (jobsData.TryGetProperty("data", out var jobsArray))
                {
                    foreach (var job in jobsArray.EnumerateArray().Take(5))
                    {
                        recommendedJobs.Add(new JobViewModel
                        {
                            Id = job.TryGetProperty("id", out var id) ? Guid.Parse(id.GetString() ?? "") : Guid.Empty,
                            Title = job.TryGetProperty("title", out var title) ? title.GetString() ?? "" : "",
                            Description = job.TryGetProperty("description", out var desc) ? desc.GetString() ?? "" : "",
                            Location = job.TryGetProperty("location", out var loc) ? loc.GetString() ?? "" : "",
                            SalaryFrom = job.TryGetProperty("salaryFrom", out var salFrom) ? salFrom.GetDecimal() : 0,
                            SalaryTo = job.TryGetProperty("salaryTo", out var salTo) ? salTo.GetDecimal() : 0,
                            JobType = job.TryGetProperty("jobType", out var type) ? type.GetString() ?? "" : "",
                            IsRemote = job.TryGetProperty("isRemote", out var remote) ? remote.GetBoolean() : false,
                            CompanyName = job.TryGetProperty("companyName", out var company) ? company.GetString() ?? "" : "",
                            PostedAt = job.TryGetProperty("postedAt", out var posted) ? DateTime.Parse(posted.GetString() ?? "") : DateTime.Now
                        });
                    }
                }
            }

            var dashboardData = new JobSeekerDashboardViewModel
            {
                Applications = applications,
                RecommendedJobs = recommendedJobs,
                TotalApplications = applications.Count,
                PendingApplications = applications.Count(a => a.Status == "Pending"),
                ApprovedApplications = applications.Count(a => a.Status == "Approved"),
                RejectedApplications = applications.Count(a => a.Status == "Rejected")
            };

            return View(dashboardData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading job seeker dashboard");
            return View(new JobSeekerDashboardViewModel());
        }
    }

    public async Task<IActionResult> BrowseJobs(string search = "", string location = "", string jobType = "")
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

            var queryParams = new List<string>();
            if (!string.IsNullOrEmpty(search)) queryParams.Add($"search={Uri.EscapeDataString(search)}");
            if (!string.IsNullOrEmpty(location)) queryParams.Add($"location={Uri.EscapeDataString(location)}");
            if (!string.IsNullOrEmpty(jobType)) queryParams.Add($"jobType={Uri.EscapeDataString(jobType)}");

            var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
            var response = await _httpClient.GetAsync($"{baseUrl}/api/jobs{queryString}");

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
                            CompanyName = job.TryGetProperty("companyName", out var company) ? company.GetString() ?? "" : "",
                            PostedAt = job.TryGetProperty("postedAt", out var posted) ? DateTime.Parse(posted.GetString() ?? "") : DateTime.Now
                        });
                    }
                }

                ViewBag.Search = search;
                ViewBag.Location = location;
                ViewBag.JobType = jobType;
                return View(jobs);
            }

            return View(new List<JobViewModel>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error browsing jobs");
            return View(new List<JobViewModel>());
        }
    }

    public async Task<IActionResult> MyApplications()
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

            var response = await _httpClient.GetAsync($"{baseUrl}/api/job-applications/my-applications");

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
                            CompanyName = app.TryGetProperty("companyName", out var company) ? company.GetString() ?? "" : "",
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
            _logger.LogError(ex, "Error loading job seeker applications");
            return View(new List<JobApplicationViewModel>());
        }
    }
}
