using Microsoft.AspNetCore.Mvc;
using HireSphere.Core.Models;
using HireSphere.Core.Repositories;

namespace HireSphere.Application.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HomeController : ControllerBase
{
    private readonly IJobEfCoreRepository _jobRepository;
    private readonly ICompanyEfCoreRepository _companyRepository;
    private readonly ICategoryEfCoreRepository _categoryRepository;

    public HomeController(
        IJobEfCoreRepository jobRepository,
        ICompanyEfCoreRepository companyRepository,
        ICategoryEfCoreRepository categoryRepository)
    {
        _jobRepository = jobRepository;
        _companyRepository = companyRepository;
        _categoryRepository = categoryRepository;
    }

    [HttpGet]
    [Route("/")]
    public async Task<ActionResult<object>> GetRoot()
    {
        try
        {
            // Get recent active jobs (last 5) for the root route
            var recentJobs = await _jobRepository.GetRecentActiveJobsAsync(5);
            
            // Get job statistics
            var totalJobs = await _jobRepository.GetTotalActiveJobsCountAsync();
            var totalCompanies = await _companyRepository.GetTotalCompaniesCountAsync();
            
            // Get companies for job summaries
            var companies = await _companyRepository.GetAllAsync();
            var companyDict = companies.ToDictionary(c => c.Id, c => c.Name);

            var jobOpportunities = recentJobs.Select(job => new
            {
                id = job.Id,
                title = job.Title,
                company = companyDict.GetValueOrDefault(job.CompanyId, "Unknown Company"),
                location = job.Location ?? "Not specified",
                salaryFrom = job.SalaryFrom ?? 0,
                salaryTo = job.SalaryTo ?? 0,
                jobType = job.JobType.ToString(),
                isRemote = job.IsRemote,
                postedAt = job.PostedAt,
                expiresAt = job.ExpiresAt
            }).ToList();

            return Ok(new
            {
                message = "Welcome to HireSphere API - Job Opportunities",
                totalJobs = totalJobs,
                totalCompanies = totalCompanies,
                recentJobOpportunities = jobOpportunities,
                endpoints = new
                {
                    allJobs = "/api/home/jobs",
                    featuredJobs = "/api/home/featured-jobs",
                    homePage = "/api/home",
                    jobs = "/api/jobs",
                    companies = "/api/companies",
                    categories = "/api/categories"
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while fetching job opportunities", details = ex.Message });
        }
    }

    [HttpGet]
    [Route("api/home")]
    public async Task<ActionResult<HomePageData>> GetHomePageData()
    {
        try
        {
            // Get recent active jobs (last 10)
            var recentJobs = await _jobRepository.GetRecentActiveJobsAsync(10);
            
            // Get job statistics
            var totalJobs = await _jobRepository.GetTotalActiveJobsCountAsync();
            var totalCompanies = await _companyRepository.GetTotalCompaniesCountAsync();
            var totalCategories = await _categoryRepository.GetTotalCategoriesCountAsync();
            
            // Get featured jobs (you can implement custom logic for featured jobs)
            var featuredJobs = await _jobRepository.GetFeaturedJobsAsync(6);
            
            // Get job categories for filtering
            var categories = await _categoryRepository.GetAllAsync();

            // Get companies for job summaries
            var companies = await _companyRepository.GetAllAsync();
            var companyDict = companies.ToDictionary(c => c.Id, c => c.Name);

            var homePageData = new HomePageData
            {
                RecentJobs = recentJobs.Select(job => new JobSummaryDto
                {
                    Id = job.Id,
                    Title = job.Title,
                    CompanyName = companyDict.GetValueOrDefault(job.CompanyId, "Unknown Company"),
                    Location = job.Location ?? "Not specified",
                    SalaryFrom = job.SalaryFrom ?? 0,
                    SalaryTo = job.SalaryTo ?? 0,
                    JobType = job.JobType.ToString(),
                    IsRemote = job.IsRemote,
                    PostedAt = job.PostedAt,
                    ExpiresAt = job.ExpiresAt,
                    CategoryName = "General" // We'll need to implement category lookup if needed
                }).ToList(),
                
                FeaturedJobs = featuredJobs.Select(job => new JobSummaryDto
                {
                    Id = job.Id,
                    Title = job.Title,
                    CompanyName = companyDict.GetValueOrDefault(job.CompanyId, "Unknown Company"),
                    Location = job.Location ?? "Not specified",
                    SalaryFrom = job.SalaryFrom ?? 0,
                    SalaryTo = job.SalaryTo ?? 0,
                    JobType = job.JobType.ToString(),
                    IsRemote = job.IsRemote,
                    PostedAt = job.PostedAt,
                    ExpiresAt = job.ExpiresAt,
                    CategoryName = "General" // We'll need to implement category lookup if needed
                }).ToList(),
                
                Statistics = new HomePageStatistics
                {
                    TotalJobs = totalJobs,
                    TotalCompanies = totalCompanies,
                    TotalCategories = totalCategories
                },
                
                Categories = categories.Select(cat => new CategorySummaryDto
                {
                    Id = cat.Id,
                    Name = cat.Name,
                    Slug = cat.Slug
                }).ToList()
            };

            return Ok(homePageData);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while fetching home page data", details = ex.Message });
        }
    }

    [HttpGet("jobs")]
    public async Task<ActionResult<IEnumerable<JobSummaryDto>>> GetJobOpportunities(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] string? location = null,
        [FromQuery] string? jobType = null,
        [FromQuery] Guid? categoryId = null,
        [FromQuery] bool? isRemote = null)
    {
        try
        {
            var jobs = await _jobRepository.GetActiveJobsWithFiltersAsync(
                page, pageSize, search, location, jobType, categoryId, isRemote);

            // Get companies for job summaries
            var companies = await _companyRepository.GetAllAsync();
            var companyDict = companies.ToDictionary(c => c.Id, c => c.Name);

            var jobSummaries = jobs.Select(job => new JobSummaryDto
            {
                Id = job.Id,
                Title = job.Title,
                CompanyName = companyDict.GetValueOrDefault(job.CompanyId, "Unknown Company"),
                Location = job.Location ?? "Not specified",
                SalaryFrom = job.SalaryFrom ?? 0,
                SalaryTo = job.SalaryTo ?? 0,
                JobType = job.JobType.ToString(),
                IsRemote = job.IsRemote,
                PostedAt = job.PostedAt,
                ExpiresAt = job.ExpiresAt,
                CategoryName = "General" // We'll need to implement category lookup if needed
            }).ToList();

            return Ok(new
            {
                jobs = jobSummaries,
                page,
                pageSize,
                totalCount = await _jobRepository.GetActiveJobsCountAsync()
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while fetching job opportunities", details = ex.Message });
        }
    }

    [HttpGet("featured-jobs")]
    public async Task<ActionResult<IEnumerable<JobSummaryDto>>> GetFeaturedJobs()
    {
        try
        {
            var featuredJobs = await _jobRepository.GetFeaturedJobsAsync(6);
            
            // Get companies for job summaries
            var companies = await _companyRepository.GetAllAsync();
            var companyDict = companies.ToDictionary(c => c.Id, c => c.Name);

            var jobSummaries = featuredJobs.Select(job => new JobSummaryDto
            {
                Id = job.Id,
                Title = job.Title,
                CompanyName = companyDict.GetValueOrDefault(job.CompanyId, "Unknown Company"),
                Location = job.Location ?? "Not specified",
                SalaryFrom = job.SalaryFrom ?? 0,
                SalaryTo = job.SalaryTo ?? 0,
                JobType = job.JobType.ToString(),
                IsRemote = job.IsRemote,
                PostedAt = job.PostedAt,
                ExpiresAt = job.ExpiresAt,
                CategoryName = "General" // We'll need to implement category lookup if needed
            }).ToList();

            return Ok(jobSummaries);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while fetching featured jobs", details = ex.Message });
        }
    }
}

// DTOs for the home page
public class HomePageData
{
    public List<JobSummaryDto> RecentJobs { get; set; } = new();
    public List<JobSummaryDto> FeaturedJobs { get; set; } = new();
    public HomePageStatistics Statistics { get; set; } = new();
    public List<CategorySummaryDto> Categories { get; set; } = new();
}

public class JobSummaryDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public decimal SalaryFrom { get; set; }
    public decimal SalaryTo { get; set; }
    public string JobType { get; set; } = string.Empty;
    public bool IsRemote { get; set; }
    public DateTime PostedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public string CategoryName { get; set; } = string.Empty;
}

public class HomePageStatistics
{
    public int TotalJobs { get; set; }
    public int TotalCompanies { get; set; }
    public int TotalCategories { get; set; }
}

public class CategorySummaryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
}
