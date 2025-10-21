using System.Text.Json;
using HireSphere.Adminpanel.Models;

namespace HireSphere.Adminpanel.Services;

public interface IAdminService
{
    Task<DashboardViewModel> GetDashboardDataAsync();
    Task<UserStatisticsViewModel> GetUserStatisticsAsync();
    Task<JobStatisticsViewModel> GetJobStatisticsAsync();
    Task<CompanyStatisticsViewModel> GetCompanyStatisticsAsync();
    Task<JobApplicationStatisticsViewModel> GetApplicationStatisticsAsync();
    Task<List<RecentUser>> GetRecentUsersAsync(int count = 5);
    Task<List<RecentJob>> GetRecentJobsAsync(int count = 5);
    Task<List<RecentCompany>> GetRecentCompaniesAsync(int count = 5);
}

public class AdminService : IAdminService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AdminService> _logger;

    public AdminService(HttpClient httpClient, IConfiguration configuration, ILogger<AdminService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<DashboardViewModel> GetDashboardDataAsync()
    {
        try
        {
            var baseUrl = _configuration["BASE_URL"];
            var dashboardData = new DashboardViewModel();

            // Get all statistics in parallel
            var userStatsTask = GetUserStatisticsAsync();
            var jobStatsTask = GetJobStatisticsAsync();
            var companyStatsTask = GetCompanyStatisticsAsync();
            var applicationStatsTask = GetApplicationStatisticsAsync();

            await Task.WhenAll(userStatsTask, jobStatsTask, companyStatsTask, applicationStatsTask);
            
            var results = new object[]
            {
                await userStatsTask,
                await jobStatsTask,
                await companyStatsTask,
                await applicationStatsTask
            };
            
            dashboardData.UserStatistics = (UserStatisticsViewModel)results[0];
            dashboardData.JobStatistics = (JobStatisticsViewModel)results[1];
            dashboardData.CompanyStatistics = (CompanyStatisticsViewModel)results[2];
            dashboardData.ApplicationStatistics = (JobApplicationStatisticsViewModel)results[3];

            return dashboardData;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting dashboard data");
            return new DashboardViewModel();
        }
    }

    public async Task<UserStatisticsViewModel> GetUserStatisticsAsync()
    {
        try
        {
            var baseUrl = _configuration["BASE_URL"];
            var response = await _httpClient.GetAsync($"{baseUrl}/api/users/statistics");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<UserStatisticsViewModel>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new UserStatisticsViewModel();
            }

            return new UserStatisticsViewModel();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user statistics");
            return new UserStatisticsViewModel();
        }
    }

    public async Task<JobStatisticsViewModel> GetJobStatisticsAsync()
    {
        try
        {
            var baseUrl = _configuration["BASE_URL"];
            var response = await _httpClient.GetAsync($"{baseUrl}/api/jobs/statistics");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<JobStatisticsViewModel>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new JobStatisticsViewModel();
            }

            return new JobStatisticsViewModel();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting job statistics");
            return new JobStatisticsViewModel();
        }
    }

    public async Task<CompanyStatisticsViewModel> GetCompanyStatisticsAsync()
    {
        try
        {
            var baseUrl = _configuration["BASE_URL"];
            var response = await _httpClient.GetAsync($"{baseUrl}/api/companies/statistics");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<CompanyStatisticsViewModel>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new CompanyStatisticsViewModel();
            }

            return new CompanyStatisticsViewModel();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting company statistics");
            return new CompanyStatisticsViewModel();
        }
    }

    public async Task<JobApplicationStatisticsViewModel> GetApplicationStatisticsAsync()
    {
        try
        {
            var baseUrl = _configuration["BASE_URL"];
            var response = await _httpClient.GetAsync($"{baseUrl}/api/jobapplications/statistics");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<JobApplicationStatisticsViewModel>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new JobApplicationStatisticsViewModel();
            }

            return new JobApplicationStatisticsViewModel();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting application statistics");
            return new JobApplicationStatisticsViewModel();
        }
    }

    public async Task<List<RecentUser>> GetRecentUsersAsync(int count = 5)
    {
        try
        {
            var baseUrl = _configuration["BASE_URL"];
            var response = await _httpClient.GetAsync($"{baseUrl}/api/users/recent?count={count}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<RecentUser>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<RecentUser>();
            }

            return new List<RecentUser>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting recent users");
            return new List<RecentUser>();
        }
    }

    public async Task<List<RecentJob>> GetRecentJobsAsync(int count = 5)
    {
        try
        {
            var baseUrl = _configuration["BASE_URL"];
            var response = await _httpClient.GetAsync($"{baseUrl}/api/jobs/recent?count={count}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<RecentJob>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<RecentJob>();
            }

            return new List<RecentJob>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting recent jobs");
            return new List<RecentJob>();
        }
    }

    public async Task<List<RecentCompany>> GetRecentCompaniesAsync(int count = 5)
    {
        try
        {
            var baseUrl = _configuration["BASE_URL"];
            var response = await _httpClient.GetAsync($"{baseUrl}/api/companies/recent?count={count}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<RecentCompany>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<RecentCompany>();
            }

            return new List<RecentCompany>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting recent companies");
            return new List<RecentCompany>();
        }
    }
}
