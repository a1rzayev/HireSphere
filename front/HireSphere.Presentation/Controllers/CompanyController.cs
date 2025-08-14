using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using HireSphere.Presentation.Models;

namespace HireSphere.Presentation.Controllers;

public class CompanyController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public CompanyController(IConfiguration configuration, HttpClient httpClient)
    {
        _configuration = configuration;
        _httpClient = httpClient;
    }

    public async Task<IActionResult> Index(string? searchTerm, string? location, int page = 1)
    {
        try
        {
            var baseUrl = _configuration["BASE_URL"];
            if (string.IsNullOrEmpty(baseUrl) || baseUrl == "baseurl")
            {
                ViewBag.ErrorMessage = "Configuration error: BASE_URL is not properly configured.";
                return View(new CompanyListViewModel());
            }

            var queryParams = new List<string>();
            if (!string.IsNullOrWhiteSpace(searchTerm))
                queryParams.Add($"search={Uri.EscapeDataString(searchTerm)}");
            if (!string.IsNullOrWhiteSpace(location))
                queryParams.Add($"location={Uri.EscapeDataString(location)}");

            var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
            var url = $"{baseUrl}/api/company{queryString}";

            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var companies = JsonSerializer.Deserialize<List<CompanyViewModel>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<CompanyViewModel>();

                var viewModel = new CompanyListViewModel
                {
                    Companies = companies,
                    TotalCount = companies.Count,
                    CurrentPage = page,
                    SearchTerm = searchTerm,
                    LocationFilter = location
                };

                return View(viewModel);
            }

            ViewBag.ErrorMessage = "Failed to fetch companies. Please try again.";
            return View(new CompanyListViewModel());
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = $"An error occurred: {ex.Message}";
            return View(new CompanyListViewModel());
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
                return RedirectToAction("Index");
            }

            var response = await _httpClient.GetAsync($"{baseUrl}/api/company/{id}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var company = JsonSerializer.Deserialize<CompanyViewModel>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (company != null)
                {
                    return View(company);
                }
            }

            ViewBag.ErrorMessage = "Company not found.";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = $"An error occurred: {ex.Message}";
            return RedirectToAction("Index");
        }
    }

    public IActionResult Create()
    {
        return View(new CompanyViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CompanyViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var baseUrl = _configuration["BASE_URL"];
            if (string.IsNullOrEmpty(baseUrl) || baseUrl == "baseurl")
            {
                ViewBag.ErrorMessage = "Configuration error: BASE_URL is not properly configured.";
                return View(model);
            }

            var accessToken = HttpContext.Session.GetString("AccessToken");
            if (string.IsNullOrEmpty(accessToken))
            {
                return RedirectToAction("Login", "Auth");
            }

            var companyRequest = new
            {
                name = model.Name.Trim(),
                description = model.Description.Trim(),
                website = !string.IsNullOrWhiteSpace(model.Website) ? model.Website.Trim() : null,
                location = !string.IsNullOrWhiteSpace(model.Location) ? model.Location.Trim() : null,
                logoUrl = !string.IsNullOrWhiteSpace(model.LogoUrl) ? model.LogoUrl.Trim() : null
            };

            var json = JsonSerializer.Serialize(companyRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/api/company");
            request.Content = content;
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Company created successfully!";
                return RedirectToAction("Index");
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            ViewBag.ErrorMessage = $"Failed to create company: {errorContent}";
            return View(model);
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = $"An error occurred while creating company: {ex.Message}";
            return View(model);
        }
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        try
        {
            var baseUrl = _configuration["BASE_URL"];
            if (string.IsNullOrEmpty(baseUrl) || baseUrl == "baseurl")
            {
                ViewBag.ErrorMessage = "Configuration error: BASE_URL is not properly configured.";
                return RedirectToAction("Index");
            }

            var accessToken = HttpContext.Session.GetString("AccessToken");
            if (string.IsNullOrEmpty(accessToken))
            {
                return RedirectToAction("Login", "Auth");
            }

            var response = await _httpClient.GetAsync($"{baseUrl}/api/company/{id}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var company = JsonSerializer.Deserialize<CompanyViewModel>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (company != null)
                {
                    return View(company);
                }
            }

            ViewBag.ErrorMessage = "Company not found.";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = $"An error occurred: {ex.Message}";
            return RedirectToAction("Index");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, CompanyViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var baseUrl = _configuration["BASE_URL"];
            if (string.IsNullOrEmpty(baseUrl) || baseUrl == "baseurl")
            {
                ViewBag.ErrorMessage = "Configuration error: BASE_URL is not properly configured.";
                return View(model);
            }

            var accessToken = HttpContext.Session.GetString("AccessToken");
            if (string.IsNullOrEmpty(accessToken))
            {
                return RedirectToAction("Login", "Auth");
            }

            var companyRequest = new
            {
                name = model.Name.Trim(),
                description = model.Description.Trim(),
                website = !string.IsNullOrWhiteSpace(model.Website) ? model.Website.Trim() : null,
                location = !string.IsNullOrWhiteSpace(model.Location) ? model.Location.Trim() : null,
                logoUrl = !string.IsNullOrWhiteSpace(model.LogoUrl) ? model.LogoUrl.Trim() : null
            };

            var json = JsonSerializer.Serialize(companyRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Put, $"{baseUrl}/api/company/{id}");
            request.Content = content;
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Company updated successfully!";
                return RedirectToAction("Index");
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            ViewBag.ErrorMessage = $"Failed to update company: {errorContent}";
            return View(model);
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = $"An error occurred while updating company: {ex.Message}";
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var baseUrl = _configuration["BASE_URL"];
            if (string.IsNullOrEmpty(baseUrl) || baseUrl == "baseurl")
            {
                TempData["ErrorMessage"] = "Configuration error: BASE_URL is not properly configured.";
                return RedirectToAction("Index");
            }

            var accessToken = HttpContext.Session.GetString("AccessToken");
            if (string.IsNullOrEmpty(accessToken))
            {
                return RedirectToAction("Login", "Auth");
            }

            var request = new HttpRequestMessage(HttpMethod.Delete, $"{baseUrl}/api/company/{id}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Company deleted successfully!";
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                TempData["ErrorMessage"] = $"Failed to delete company: {errorContent}";
            }

            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"An error occurred while deleting company: {ex.Message}";
            return RedirectToAction("Index");
        }
    }
}
