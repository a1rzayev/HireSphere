using Microsoft.AspNetCore.Mvc;
using HireSphere.Presentation.Models;
using System.Text;
using System.Text.Json;

namespace HireSphere.Presentation.Controllers;

public class JobsController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private readonly ILogger<JobsController> _logger;

    public JobsController(IConfiguration configuration, HttpClient httpClient, ILogger<JobsController> logger)
    {
        _configuration = configuration;
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<IActionResult> Index(string? search, string? location, string? jobType, string? category, bool? isRemote, int page = 1, int pageSize = 10)
    {
        try
        {
            var baseUrl = _configuration["BASE_URL"];
            if (string.IsNullOrEmpty(baseUrl) || baseUrl == "baseurl")
            {
                ViewBag.ErrorMessage = "Configuration error: BASE_URL is not properly configured.";
                return View(new JobsListViewModel());
            }

            var queryParams = new List<string>();
            if (!string.IsNullOrWhiteSpace(search))
                queryParams.Add($"search={Uri.EscapeDataString(search)}");
            if (!string.IsNullOrWhiteSpace(location))
                queryParams.Add($"location={Uri.EscapeDataString(location)}");
            if (!string.IsNullOrWhiteSpace(jobType))
                queryParams.Add($"jobType={Uri.EscapeDataString(jobType)}");
            if (!string.IsNullOrWhiteSpace(category))
                queryParams.Add($"category={Uri.EscapeDataString(category)}");
            if (isRemote.HasValue)
                queryParams.Add($"isRemote={isRemote.Value}");
            if (page > 1)
                queryParams.Add($"page={page}");
            if (pageSize != 10)
                queryParams.Add($"pageSize={pageSize}");

            var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
            var url = $"{baseUrl}/api/home/jobs{queryString}";

            _logger.LogInformation($"Making request to: {url}");
            var response = await _httpClient.GetAsync(url);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"API Response: {content}");
                
                try
                {
                    var jobsData = JsonSerializer.Deserialize<JsonElement>(content);

                    var jobs = new List<JobViewModel>();
                    if (jobsData.TryGetProperty("jobs", out var jobsArray))
                    {
                        foreach (var job in jobsArray.EnumerateArray())
                        {
                            try
                            {
                                jobs.Add(new JobViewModel
                                {
                                    Id = job.TryGetProperty("id", out var id) ? Guid.Parse(id.GetString() ?? "") : Guid.Empty,
                                    Title = job.TryGetProperty("title", out var title) ? title.GetString() ?? "" : "",
                                    CompanyName = job.TryGetProperty("companyName", out var company) ? company.GetString() ?? "" : "",
                                    Location = job.TryGetProperty("location", out var loc) ? loc.GetString() ?? "" : "",
                                    SalaryFrom = job.TryGetProperty("salaryFrom", out var salFrom) ? salFrom.GetDecimal() : 0,
                                    SalaryTo = job.TryGetProperty("salaryTo", out var salTo) ? salTo.GetDecimal() : 0,
                                    JobType = job.TryGetProperty("jobType", out var type) ? type.GetString() ?? "" : "",
                                    IsRemote = job.TryGetProperty("isRemote", out var remote) ? remote.GetBoolean() : false,
                                    PostedAt = job.TryGetProperty("postedAt", out var posted) ? DateTime.Parse(posted.GetString() ?? "") : DateTime.Now,
                                    ExpiresAt = job.TryGetProperty("expiresAt", out var expires) ? DateTime.Parse(expires.GetString() ?? "") : DateTime.Now.AddDays(30),
                                    CategoryName = job.TryGetProperty("categoryName", out var cat) ? cat.GetString() ?? "" : ""
                                });
                            }
                            catch (Exception jobEx)
                            {
                                _logger.LogError(jobEx, "Error parsing individual job");
                            }
                        }
                    }

                    var totalCount = jobsData.TryGetProperty("totalCount", out var total) ? total.GetInt32() : 0;
                    var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                    var viewModel = new JobsListViewModel
                    {
                        Jobs = jobs,
                        SearchTerm = search ?? "",
                        Location = location ?? "",
                        JobType = jobType ?? "",
                        Category = category ?? "",
                        IsRemote = isRemote ?? false,
                        CurrentPage = page,
                        PageSize = pageSize,
                        TotalPages = totalPages,
                        TotalCount = totalCount
                    };

                    _logger.LogInformation($"Successfully loaded {jobs.Count} jobs");
                    return View(viewModel);
                }
                catch (Exception parseEx)
                {
                    _logger.LogError(parseEx, "Error parsing API response");
                    ViewBag.ErrorMessage = "Error parsing job data. Please try again later.";
                    return View(new JobsListViewModel());
                }
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError($"API request failed with status {response.StatusCode}: {errorContent}");
                ViewBag.ErrorMessage = $"Failed to load jobs. API returned status {response.StatusCode}. Please try again later.";
                return View(new JobsListViewModel());
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading jobs");
            ViewBag.ErrorMessage = "An error occurred while loading jobs.";
            return View(new JobsListViewModel());
        }
    }

    public async Task<IActionResult> Details(Guid id)
    {
        try
        {
            var baseUrl = _configuration["BASE_URL"];
            if (string.IsNullOrEmpty(baseUrl) || baseUrl == "baseurl")
            {
                ViewBag.ErrorMessage = "Configuration error: BASE_URL is not properly configured.";
                return View(new JobViewModel());
            }

            var response = await _httpClient.GetAsync($"{baseUrl}/api/jobs/{id}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var jobData = JsonSerializer.Deserialize<JsonElement>(content);

                var job = new JobViewModel
                {
                    Id = jobData.TryGetProperty("id", out var idProp) ? Guid.Parse(idProp.GetString() ?? "") : Guid.Empty,
                    Title = jobData.TryGetProperty("title", out var title) ? title.GetString() ?? "" : "",
                    Description = jobData.TryGetProperty("description", out var desc) ? desc.GetString() ?? "" : "",
                    Requirements = jobData.TryGetProperty("requirements", out var req) ? req.GetString() ?? "" : "",
                    CompanyName = jobData.TryGetProperty("companyName", out var company) ? company.GetString() ?? "" : "",
                    Location = jobData.TryGetProperty("location", out var loc) ? loc.GetString() ?? "" : "",
                    SalaryFrom = jobData.TryGetProperty("salaryFrom", out var salFrom) ? salFrom.GetDecimal() : 0,
                    SalaryTo = jobData.TryGetProperty("salaryTo", out var salTo) ? salTo.GetDecimal() : 0,
                    JobType = jobData.TryGetProperty("jobType", out var type) ? type.GetString() ?? "" : "",
                    IsRemote = jobData.TryGetProperty("isRemote", out var remote) ? remote.GetBoolean() : false,
                    PostedAt = jobData.TryGetProperty("postedAt", out var posted) ? DateTime.Parse(posted.GetString() ?? "") : DateTime.Now,
                    ExpiresAt = jobData.TryGetProperty("expiresAt", out var expires) ? DateTime.Parse(expires.GetString() ?? "") : DateTime.Now.AddDays(30),
                    CategoryName = jobData.TryGetProperty("categoryName", out var cat) ? cat.GetString() ?? "" : "",
                    Tags = jobData.TryGetProperty("tags", out var tags) ? 
                        tags.EnumerateArray().Select(t => t.GetString() ?? "").ToList() : new List<string>()
                };

                return View(job);
            }
            else
            {
                ViewBag.ErrorMessage = "Job not found.";
                return View(new JobViewModel());
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading job details");
            ViewBag.ErrorMessage = "An error occurred while loading job details.";
            return View(new JobViewModel());
        }
    }

    public async Task<IActionResult> Search(string? search, string? location, string? jobType, string? category, bool? isRemote)
    {
        return RedirectToAction("Index", new { search, location, jobType, category, isRemote });
    }
}
