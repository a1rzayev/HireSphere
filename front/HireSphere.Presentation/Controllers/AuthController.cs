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

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                ViewBag.ErrorMessage = "First name is required.";
                return View(model);
            }

            if (string.IsNullOrWhiteSpace(model.Surname))
            {
                ViewBag.ErrorMessage = "Last name is required.";
                return View(model);
            }

            if (string.IsNullOrWhiteSpace(model.Email))
            {
                ViewBag.ErrorMessage = "Email is required.";
                return View(model);
            }

            if (!IsValidEmail(model.Email))
            {
                ViewBag.ErrorMessage = "Please enter a valid email address.";
                return View(model);
            }

            if (string.IsNullOrWhiteSpace(model.Password))
            {
                ViewBag.ErrorMessage = "Password is required.";
                return View(model);
            }

            if (model.Password.Length < 6)
            {
                ViewBag.ErrorMessage = "Password must be at least 6 characters long.";
                return View(model);
            }

            if (model.Password != model.ConfirmPassword)
            {
                ViewBag.ErrorMessage = "Passwords do not match.";
                return View(model);
            }

            var baseUrl = _configuration["BASE_URL"];
            
            if (string.IsNullOrEmpty(baseUrl) || baseUrl == "baseurl")
            {
                ViewBag.ErrorMessage = "Configuration error: BASE_URL is not properly configured.";
                return View(model);
            }
            var registerRequest = new
            {
                Name = model.Name.Trim(),
                Surname = model.Surname.Trim(),
                Email = model.Email.Trim(),
                Password = model.Password,
                ConfirmPassword = model.ConfirmPassword
            };

            var json = JsonSerializer.Serialize(registerRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{baseUrl}/api/auth/register", content);

            if (response.IsSuccessStatusCode)
            {
                ViewBag.SuccessMessage = "Registration successful! Please log in with your new account.";
                return RedirectToAction("Login");
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            ViewBag.ErrorMessage = $"Registration failed: {errorContent}";
            return View(model);
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = $"An error occurred during registration: {ex.Message}";
            return View(model);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(model.Email))
            {
                ViewBag.ErrorMessage = "Email is required.";
                return View(model);
            }

            if (string.IsNullOrWhiteSpace(model.Password))
            {
                ViewBag.ErrorMessage = "Password is required.";
                return View(model);
            }

            if (!IsValidEmail(model.Email))
            {
                ViewBag.ErrorMessage = "Please enter a valid email address.";
                return View(model);
            }

            var baseUrl = _configuration["BASE_URL"];
            
            if (string.IsNullOrEmpty(baseUrl) || baseUrl == "baseurl")
            {
                ViewBag.ErrorMessage = "Configuration error: BASE_URL is not properly configured.";
                return View(model);
            }
            
            var loginRequest = new
            {
                Email = model.Email.Trim(),
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
                    string token = accessToken.ValueKind == JsonValueKind.String 
                        ? accessToken.GetString() ?? "" 
                        : accessToken.ToString();
                    HttpContext.Session.SetString("AccessToken", token);
                }
                
                if (authResponse.TryGetProperty("refreshToken", out var refreshToken))
                {
                    string refresh = refreshToken.ValueKind == JsonValueKind.String 
                        ? refreshToken.GetString() ?? "" 
                        : refreshToken.ToString();
                    HttpContext.Session.SetString("RefreshToken", refresh);
                }
                
                if (authResponse.TryGetProperty("user", out var userElement))
                {
                    if (userElement.TryGetProperty("id", out var id))
                    {
                        string userId = id.ValueKind == JsonValueKind.String 
                            ? id.GetString() ?? "" 
                            : id.ToString();
                        HttpContext.Session.SetString("UserId", userId);
                    }
                    
                    if (userElement.TryGetProperty("name", out var name))
                    {
                        string nameValue = name.ValueKind == JsonValueKind.String 
                            ? name.GetString() ?? "" 
                            : name.ToString();
                        
                        if (userElement.TryGetProperty("surname", out var surname))
                        {
                            string surnameValue = surname.ValueKind == JsonValueKind.String 
                                ? surname.GetString() ?? "" 
                                : surname.ToString();
                            
                            string userName = $"{nameValue} {surnameValue}".Trim();
                            HttpContext.Session.SetString("UserName", userName);
                        }
                        else
                        {
                            HttpContext.Session.SetString("UserName", nameValue);
                        }
                    }
                    
                    if (userElement.TryGetProperty("email", out var email))
                    {
                        string emailValue = email.ValueKind == JsonValueKind.String 
                            ? email.GetString() ?? "" 
                            : email.ToString();
                        HttpContext.Session.SetString("UserEmail", emailValue);
                    }
                    
                    if (userElement.TryGetProperty("role", out var role))
                    {
                        string roleValue = role.ValueKind == JsonValueKind.String 
                            ? role.GetString() ?? "" 
                            : role.ToString();
                        HttpContext.Session.SetString("UserRole", roleValue);
                    }
                }

                return RedirectToAction("Index", "Home");
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            ViewBag.ErrorMessage = $"Login failed: {errorContent}";
            return View(model);
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = $"An error occurred during login: {ex.Message}";
            return View(model);
        }
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
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
