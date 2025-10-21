using Microsoft.AspNetCore.Mvc;
using HireSphere.Adminpanel.Models;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using HireSphere.Adminpanel.Filters;

namespace HireSphere.Adminpanel.Controllers;

[ServiceFilter(typeof(SessionAuthorizationFilter))]
public class JobController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private readonly ILogger<JobController> _logger;

    public JobController(IConfiguration configuration, HttpClient httpClient, ILogger<JobController> logger)
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
            var response = await _httpClient.GetAsync($"{baseUrl}/api/jobs{queryString}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var jobs = JsonSerializer.Deserialize<List<JobViewModel>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                var viewModel = new JobIndexViewModel
                {
                    Jobs = jobs ?? new List<JobViewModel>(),
                    CurrentPage = page,
                    PageSize = pageSize,
                    SearchTerm = search,
                    StatusFilter = status,
                    TotalPages = (int)Math.Ceiling((double)(jobs?.Count ?? 0) / pageSize)
                };

                return View(viewModel);
            }

            ViewBag.ErrorMessage = "Failed to load jobs.";
            return View(new JobIndexViewModel { Jobs = new List<JobViewModel>() });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading jobs");
            ViewBag.ErrorMessage = "An error occurred while loading jobs.";
            return View(new JobIndexViewModel { Jobs = new List<JobViewModel>() });
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

            var response = await _httpClient.GetAsync($"{baseUrl}/api/jobs/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var job = JsonSerializer.Deserialize<JobViewModel>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return View(job);
            }

            ViewBag.ErrorMessage = "Job not found.";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading job details");
            ViewBag.ErrorMessage = "An error occurred while loading job details.";
            return RedirectToAction("Index");
        }
    }

    public async Task<IActionResult> Edit(Guid id)
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

            var response = await _httpClient.GetAsync($"{baseUrl}/api/jobs/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var job = JsonSerializer.Deserialize<JobViewModel>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return View(job);
            }

            ViewBag.ErrorMessage = "Job not found.";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading job for edit");
            ViewBag.ErrorMessage = "An error occurred while loading job.";
            return RedirectToAction("Index");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Edit(JobViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var baseUrl = _configuration["BASE_URL"];
            var accessToken = HttpContext.Session.GetString("AccessToken");

            if (string.IsNullOrEmpty(accessToken))
            {
                return RedirectToAction("Login", "Auth");
            }

            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var json = JsonSerializer.Serialize(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"{baseUrl}/api/jobs/{model.Id}", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Job updated successfully.";
                return RedirectToAction("Index");
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            ViewBag.ErrorMessage = "Failed to update job.";
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating job");
            ViewBag.ErrorMessage = "An error occurred while updating job.";
            return View(model);
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

            var response = await _httpClient.DeleteAsync($"{baseUrl}/api/jobs/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Job deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete job.";
            }

            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting job");
            TempData["ErrorMessage"] = "An error occurred while deleting job.";
            return RedirectToAction("Index");
        }
    }

    [HttpPost]
    public async Task<IActionResult> ToggleStatus(Guid id)
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

            var response = await _httpClient.PostAsync($"{baseUrl}/api/jobs/{id}/toggle-status", null);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Job status updated successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to update job status.";
            }

            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling job status");
            TempData["ErrorMessage"] = "An error occurred while updating job status.";
            return RedirectToAction("Index");
        }
    }
}
