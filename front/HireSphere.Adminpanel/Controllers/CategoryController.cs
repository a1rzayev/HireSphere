using Microsoft.AspNetCore.Mvc;
using HireSphere.Adminpanel.Models;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using HireSphere.Adminpanel.Filters;

namespace HireSphere.Adminpanel.Controllers;

[ServiceFilter(typeof(SessionAuthorizationFilter))]
public class CategoryController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private readonly ILogger<CategoryController> _logger;

    public CategoryController(IConfiguration configuration, HttpClient httpClient, ILogger<CategoryController> logger)
    {
        _configuration = configuration;
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
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

            var response = await _httpClient.GetAsync($"{baseUrl}/api/categories");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var categories = JsonSerializer.Deserialize<List<CategoryViewModel>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return View(categories ?? new List<CategoryViewModel>());
            }

            ViewBag.ErrorMessage = "Failed to load categories.";
            return View(new List<CategoryViewModel>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading categories");
            ViewBag.ErrorMessage = "An error occurred while loading categories.";
            return View(new List<CategoryViewModel>());
        }
    }

    public IActionResult Create()
    {
        return View(new CategoryViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Create(CategoryViewModel model)
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

            var response = await _httpClient.PostAsync($"{baseUrl}/api/categories", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Category created successfully.";
                return RedirectToAction("Index");
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            ViewBag.ErrorMessage = "Failed to create category.";
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating category");
            ViewBag.ErrorMessage = "An error occurred while creating category.";
            return View(model);
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

            var response = await _httpClient.GetAsync($"{baseUrl}/api/categories/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var category = JsonSerializer.Deserialize<CategoryViewModel>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return View(category);
            }

            ViewBag.ErrorMessage = "Category not found.";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading category for edit");
            ViewBag.ErrorMessage = "An error occurred while loading category.";
            return RedirectToAction("Index");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Edit(CategoryViewModel model)
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

            var response = await _httpClient.PutAsync($"{baseUrl}/api/categories/{model.Id}", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Category updated successfully.";
                return RedirectToAction("Index");
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            ViewBag.ErrorMessage = "Failed to update category.";
            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating category");
            ViewBag.ErrorMessage = "An error occurred while updating category.";
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

            var response = await _httpClient.DeleteAsync($"{baseUrl}/api/categories/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Category deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete category.";
            }

            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting category");
            TempData["ErrorMessage"] = "An error occurred while deleting category.";
            return RedirectToAction("Index");
        }
    }
}
