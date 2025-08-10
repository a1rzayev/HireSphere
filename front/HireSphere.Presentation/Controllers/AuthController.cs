using Microsoft.AspNetCore.Mvc;
using HireSphere.Presentation.Models;
using System.Text;
using System.Text.Json;

namespace HireSphere.Presentation.Controllers;

public class AuthController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public AuthController(IConfiguration configuration, HttpClient httpClient)
    {
        _configuration = configuration;
        _httpClient = httpClient;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        try
        {
            var baseUrl = _configuration["BASE_URL"];
            var loginRequest = new
            {
                Email = model.Email,
                Password = model.Password
            };

            var json = JsonSerializer.Serialize(loginRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{baseUrl}/api/auth/login", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var authResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);

                if (authResponse.TryGetProperty("accessToken", out var accessToken))
                {
                    HttpContext.Session.SetString("AccessToken", accessToken.GetString() ?? "");
                }
                
                if (authResponse.TryGetProperty("refreshToken", out var refreshToken))
                {
                    HttpContext.Session.SetString("RefreshToken", refreshToken.GetString() ?? "");
                }
                
                if (authResponse.TryGetProperty("user", out var userElement))
                {
                    if (userElement.TryGetProperty("id", out var id))
                    {
                        HttpContext.Session.SetString("UserId", id.GetString() ?? "");
                    }
                    
                    if (userElement.TryGetProperty("name", out var name) && userElement.TryGetProperty("surname", out var surname))
                    {
                        HttpContext.Session.SetString("UserName", $"{name.GetString()} {surname.GetString()}");
                    }
                    
                    if (userElement.TryGetProperty("email", out var email))
                    {
                        HttpContext.Session.SetString("UserEmail", email.GetString() ?? "");
                    }
                    
                    if (userElement.TryGetProperty("role", out var role))
                    {
                        HttpContext.Session.SetString("UserRole", role.GetString() ?? "");
                    }
                }

                return RedirectToAction("Index", "Home");
            }

            ViewBag.ErrorMessage = "Login failed. Please check your credentials.";
            return View(model);
        }
        catch (Exception)
        {
            ViewBag.ErrorMessage = "An error occurred during login.";
            return View(model);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        try
        {
            var refreshToken = HttpContext.Session.GetString("RefreshToken");
            
            if (!string.IsNullOrEmpty(refreshToken))
            {
                var baseUrl = _configuration["BASE_URL"];
                var json = JsonSerializer.Serialize(refreshToken);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                await _httpClient.PostAsync($"{baseUrl}/api/auth/logout", content);
            }

            HttpContext.Session.Clear();
        }
        catch (Exception)
        {
            //Logging in the future
        }

        return RedirectToAction("Index", "Home");
    }
}
