using Microsoft.AspNetCore.Mvc;
using HireSphere.Adminpanel.Models;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using HireSphere.Adminpanel.Filters;

namespace HireSphere.Adminpanel.Controllers;

[ServiceFilter(typeof(SessionAuthorizationFilter))]
public class JobApplicationController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private readonly ILogger<JobApplicationController> _logger;

    public JobApplicationController(IConfiguration configuration, HttpClient httpClient, ILogger<JobApplicationController> logger)
    {
        _configuration = configuration;
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string search = "", string status = "")
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
            if (page > 1) queryParams.Add($"page={page}");
            if (pageSize != 10) queryParams.Add($"pageSize={pageSize}");
            if (!string.IsNullOrEmpty(search)) queryParams.Add($"search={Uri.EscapeDataString(search)}");
            if (!string.IsNullOrEmpty(status)) queryParams.Add($"status={Uri.EscapeDataString(status)}");

            var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
            var response = await _httpClient.GetAsync($"{baseUrl}/api/jobapplications{queryString}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var applications = JsonSerializer.Deserialize<List<JobApplicationViewModel>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                var viewModel = new JobApplicationIndexViewModel
                {
                    Applications = applications ?? new List<JobApplicationViewModel>(),
                    CurrentPage = page,
                    PageSize = pageSize,
                    SearchTerm = search,
                    StatusFilter = status,
                    TotalPages = (int)Math.Ceiling((double)(applications?.Count ?? 0) / pageSize)
                };

                return View(viewModel);
            }

            ViewBag.ErrorMessage = "Failed to load job applications.";
            return View(new JobApplicationIndexViewModel { Applications = new List<JobApplicationViewModel>() });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading job applications");
            ViewBag.ErrorMessage = "An error occurred while loading job applications.";
            return View(new JobApplicationIndexViewModel { Applications = new List<JobApplicationViewModel>() });
        }
    }

    public async Task<IActionResult> Details(Guid id)
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

            var response = await _httpClient.GetAsync($"{baseUrl}/api/jobapplications/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var application = JsonSerializer.Deserialize<JobApplicationViewModel>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return View(application);
            }

            ViewBag.ErrorMessage = "Job application not found.";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading job application details");
            ViewBag.ErrorMessage = "An error occurred while loading job application details.";
            return RedirectToAction("Index");
        }
    }

    [HttpPost]
    public async Task<IActionResult> UpdateStatus(Guid id, string status)
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

            var request = new { Status = status };
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"{baseUrl}/api/jobapplications/{id}/status", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Application status updated successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to update application status.";
            }

            return RedirectToAction("Details", new { id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating application status");
            TempData["ErrorMessage"] = "An error occurred while updating application status.";
            return RedirectToAction("Details", new { id });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Delete(Guid id)
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

            var response = await _httpClient.DeleteAsync($"{baseUrl}/api/jobapplications/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Job application deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete job application.";
            }

            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting job application");
            TempData["ErrorMessage"] = "An error occurred while deleting job application.";
            return RedirectToAction("Index");
        }
    }

    public async Task<IActionResult> Statistics()
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

            var response = await _httpClient.GetAsync($"{baseUrl}/api/jobapplications/statistics");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var statistics = JsonSerializer.Deserialize<JobApplicationStatisticsViewModel>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return View(statistics);
            }

            ViewBag.ErrorMessage = "Failed to load statistics.";
            return View(new JobApplicationStatisticsViewModel());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading job application statistics");
            ViewBag.ErrorMessage = "An error occurred while loading statistics.";
            return View(new JobApplicationStatisticsViewModel());
        }
    }
}
