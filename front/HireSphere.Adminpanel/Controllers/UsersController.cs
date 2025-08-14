using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace HireSphere.Adminpanel.Controllers;

public class UsersController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public UsersController(IConfiguration configuration, HttpClient httpClient)
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
                return View();
            }

            var request = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}/api/user");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var users = JsonSerializer.Deserialize<List<dynamic>>(content);
                ViewBag.Users = users;
                ViewBag.SuccessMessage = $"Successfully loaded {users?.Count ?? 0} users.";
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                ViewBag.ErrorMessage = $"Failed to fetch users: {response.StatusCode} - {errorContent}";
            }
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = $"An error occurred while fetching users: {ex.Message}";
        }

        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
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
                return View();
            }

            var request = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}/api/user/{id}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var user = JsonSerializer.Deserialize<dynamic>(content);
                ViewBag.User = user;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                ViewBag.ErrorMessage = $"Failed to fetch user: {response.StatusCode} - {errorContent}";
            }
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = $"An error occurred while fetching user: {ex.Message}";
        }

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Delete(Guid id)
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
                TempData["ErrorMessage"] = "Configuration error: BASE_URL is not properly configured.";
                return RedirectToAction("Index");
            }

            var request = new HttpRequestMessage(HttpMethod.Delete, $"{baseUrl}/api/user/{id}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "User deleted successfully.";
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                TempData["ErrorMessage"] = $"Failed to delete user: {response.StatusCode} - {errorContent}";
            }
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"An error occurred while deleting user: {ex.Message}";
        }

        return RedirectToAction("Index");
    }
}
