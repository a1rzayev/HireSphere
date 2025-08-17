using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using HireSphere.Adminpanel.Models;
using Microsoft.AspNetCore.Authorization;
using HireSphere.Adminpanel.Filters;

namespace HireSphere.Adminpanel.Controllers;

[ServiceFilter(typeof(SessionAuthorizationFilter))]
public class CompaniesController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public CompaniesController(IConfiguration configuration, HttpClient httpClient)
    {
        _configuration = configuration;
        _httpClient = httpClient;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        try
        {
            var accessToken = HttpContext.Session.GetString("AccessToken");
            if (string.IsNullOrEmpty(accessToken))
            {
                return RedirectToAction("Login", "Auth");
            }

            var baseUrl = _configuration["BASE_URL"];
            if (string.IsNullOrEmpty(baseUrl) || baseUrl == "baseurl")
            {
                ViewBag.ErrorMessage = "Configuration error: BASE_URL is not properly configured.";
                return View(new List<CompanyViewModel>());
            }

            var request = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}/api/company");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(content))
                {
                    try
                    {
                        var companies = JsonSerializer.Deserialize<List<Company>>(content, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                        
                        if (companies != null)
                        {
                            var companyViewModels = companies.Select(c => new CompanyViewModel
                            {
                                Id = c.Id,
                                OwnerUserId = c.OwnerUserId,
                                Name = c.Name,
                                Description = c.Description,
                                Website = c.Website,
                                LogoUrl = c.LogoUrl,
                                Location = c.Location,
                                CreatedAt = c.CreatedAt
                            }).ToList();
                            
                            ViewBag.SuccessMessage = $"Successfully loaded {companyViewModels.Count} companies.";
                            return View(companyViewModels);
                        }
                        else
                        {
                            ViewBag.ErrorMessage = "Failed to deserialize companies data";
                            return View(new List<CompanyViewModel>());
                        }
                    }
                    catch (JsonException ex)
                    {
                        ViewBag.ErrorMessage = $"Failed to parse companies data: {ex.Message}";
                        return View(new List<CompanyViewModel>());
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = "API returned empty response";
                    return View(new List<CompanyViewModel>());
                }
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                ViewBag.ErrorMessage = $"Failed to fetch companies: {response.StatusCode} - {errorContent}";
                return View(new List<CompanyViewModel>());
            }
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = $"An error occurred while fetching companies: {ex.Message}";
            return View(new List<CompanyViewModel>());
        }
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        if (id == Guid.Empty)
        {
            ViewBag.ErrorMessage = "Invalid company ID provided.";
            return View();
        }

        try
        {
            var accessToken = HttpContext.Session.GetString("AccessToken");
            if (string.IsNullOrEmpty(accessToken))
            {
                return RedirectToAction("Login", "Auth");
            }

            var baseUrl = _configuration["BASE_URL"];
            if (string.IsNullOrEmpty(baseUrl) || baseUrl == "baseurl")
            {
                ViewBag.ErrorMessage = "Configuration error: BASE_URL is not properly configured.";
                return View();
            }

            var request = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}/api/company/{id}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(content))
                {
                    try
                    {
                        var company = JsonSerializer.Deserialize<Company>(content, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                        
                        if (company != null)
                        {
                            var companyViewModel = new CompanyViewModel
                            {
                                Id = company.Id,
                                OwnerUserId = company.OwnerUserId,
                                Name = company.Name,
                                Description = company.Description,
                                Website = company.Website,
                                LogoUrl = company.LogoUrl,
                                Location = company.Location,
                                CreatedAt = company.CreatedAt
                            };
                            
                            return View(companyViewModel);
                        }
                        else
                        {
                            ViewBag.ErrorMessage = "Failed to parse company data from the API response.";
                        }
                    }
                    catch (JsonException ex)
                    {
                        ViewBag.ErrorMessage = $"Failed to parse company data: {ex.Message}";
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = "API returned empty response for company details.";
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                ViewBag.ErrorMessage = "Company not found. The requested company may have been deleted or never existed.";
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                ViewBag.ErrorMessage = $"Failed to fetch company: {response.StatusCode} - {errorContent}";
            }
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = $"An error occurred while fetching company: {ex.Message}";
        }

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Delete(Guid id)
    {
        if (id == Guid.Empty)
        {
            TempData["ErrorMessage"] = "Invalid company ID provided.";
            return RedirectToAction("Index");
        }

        try
        {
            var accessToken = HttpContext.Session.GetString("AccessToken");
            if (string.IsNullOrEmpty(accessToken))
            {
                TempData["ErrorMessage"] = "Access token not found. Please log in again.";
                return RedirectToAction("Index");
            }

            var baseUrl = _configuration["BASE_URL"];
            if (string.IsNullOrEmpty(baseUrl) || baseUrl == "baseurl")
            {
                TempData["ErrorMessage"] = "Configuration error: BASE_URL is not properly configured.";
                return RedirectToAction("Index");
            }

            var request = new HttpRequestMessage(HttpMethod.Delete, $"{baseUrl}/api/company/{id}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Company deleted successfully.";
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                TempData["ErrorMessage"] = "Company not found. It may have already been deleted.";
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                TempData["ErrorMessage"] = $"Failed to delete company: {response.StatusCode} - {errorContent}";
            }
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"An error occurred while deleting company: {ex.Message}";
        }

        return RedirectToAction("Index");
    }
}
