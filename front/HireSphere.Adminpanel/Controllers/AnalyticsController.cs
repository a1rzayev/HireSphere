using Microsoft.AspNetCore.Mvc;
using HireSphere.Adminpanel.Models;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using HireSphere.Adminpanel.Filters;

namespace HireSphere.Adminpanel.Controllers;

[ServiceFilter(typeof(SessionAuthorizationFilter))]
public class AnalyticsController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private readonly ILogger<AnalyticsController> _logger;

    public AnalyticsController(IConfiguration configuration, HttpClient httpClient, ILogger<AnalyticsController> logger)
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

            // Get dashboard statistics
            var dashboardData = new DashboardViewModel();

            // Get user statistics
            var usersResponse = await _httpClient.GetAsync($"{baseUrl}/api/users/statistics");
            if (usersResponse.IsSuccessStatusCode)
            {
                var usersContent = await usersResponse.Content.ReadAsStringAsync();
                var userStats = JsonSerializer.Deserialize<UserStatisticsViewModel>(usersContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                dashboardData.UserStatistics = userStats ?? new UserStatisticsViewModel();
            }

            // Get job statistics
            var jobsResponse = await _httpClient.GetAsync($"{baseUrl}/api/jobs/statistics");
            if (jobsResponse.IsSuccessStatusCode)
            {
                var jobsContent = await jobsResponse.Content.ReadAsStringAsync();
                var jobStats = JsonSerializer.Deserialize<JobStatisticsViewModel>(jobsContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                dashboardData.JobStatistics = jobStats ?? new JobStatisticsViewModel();
            }

            // Get company statistics
            var companiesResponse = await _httpClient.GetAsync($"{baseUrl}/api/companies/statistics");
            if (companiesResponse.IsSuccessStatusCode)
            {
                var companiesContent = await companiesResponse.Content.ReadAsStringAsync();
                var companyStats = JsonSerializer.Deserialize<CompanyStatisticsViewModel>(companiesContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                dashboardData.CompanyStatistics = companyStats ?? new CompanyStatisticsViewModel();
            }

            // Get application statistics
            var applicationsResponse = await _httpClient.GetAsync($"{baseUrl}/api/jobapplications/statistics");
            if (applicationsResponse.IsSuccessStatusCode)
            {
                var applicationsContent = await applicationsResponse.Content.ReadAsStringAsync();
                var applicationStats = JsonSerializer.Deserialize<JobApplicationStatisticsViewModel>(applicationsContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                dashboardData.ApplicationStatistics = applicationStats ?? new JobApplicationStatisticsViewModel();
            }

            return View(dashboardData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading dashboard data");
            ViewBag.ErrorMessage = "An error occurred while loading dashboard data.";
            return View(new DashboardViewModel());
        }
    }

    public async Task<IActionResult> Users()
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

            var response = await _httpClient.GetAsync($"{baseUrl}/api/users/analytics");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var analytics = JsonSerializer.Deserialize<UserAnalyticsViewModel>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return View(analytics);
            }

            ViewBag.ErrorMessage = "Failed to load user analytics.";
            return View(new UserAnalyticsViewModel());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user analytics");
            ViewBag.ErrorMessage = "An error occurred while loading user analytics.";
            return View(new UserAnalyticsViewModel());
        }
    }

    public async Task<IActionResult> Jobs()
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

            var response = await _httpClient.GetAsync($"{baseUrl}/api/jobs/analytics");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var analytics = JsonSerializer.Deserialize<JobAnalyticsViewModel>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return View(analytics);
            }

            ViewBag.ErrorMessage = "Failed to load job analytics.";
            return View(new JobAnalyticsViewModel());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading job analytics");
            ViewBag.ErrorMessage = "An error occurred while loading job analytics.";
            return View(new JobAnalyticsViewModel());
        }
    }

    public async Task<IActionResult> Companies()
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

            var response = await _httpClient.GetAsync($"{baseUrl}/api/companies/analytics");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var analytics = JsonSerializer.Deserialize<CompanyAnalyticsViewModel>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return View(analytics);
            }

            ViewBag.ErrorMessage = "Failed to load company analytics.";
            return View(new CompanyAnalyticsViewModel());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading company analytics");
            ViewBag.ErrorMessage = "An error occurred while loading company analytics.";
            return View(new CompanyAnalyticsViewModel());
        }
    }
}
