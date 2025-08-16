using Microsoft.AspNetCore.Mvc;
using HireSphere.Adminpanel.Models;
using System.Text;
using System.Text.Json;

namespace HireSphere.Adminpanel.Controllers;

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
            
            System.Diagnostics.Debug.WriteLine($"Using BASE_URL: {baseUrl}");
            System.Diagnostics.Debug.WriteLine($"Environment: {_configuration["ASPNETCORE_ENVIRONMENT"]}");
            
            if (string.IsNullOrEmpty(baseUrl) || baseUrl == "baseurl")
            {
                ViewBag.ErrorMessage = "Configuration error: BASE_URL is not properly configured. Please set BASE_URL in appsettings.json";
                return View(model);
            }
            
            var loginRequest = new
            {
                Email = model.Email.Trim(),
                Password = model.Password,
                Role = 0 
            };

            var json = JsonSerializer.Serialize(loginRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var healthCheckResponse = await _httpClient.GetAsync($"{baseUrl}/swagger");
                if (healthCheckResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    var rootResponse = await _httpClient.GetAsync(baseUrl);
                    if (rootResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        ViewBag.ErrorMessage = $"Cannot connect to API at {baseUrl}. Please check if the API is running and the URL is correct.";
                        return View(model);
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                ViewBag.ErrorMessage = $"Cannot connect to API at {baseUrl}: {ex.Message}. Please check if the API is running and the URL is correct.";
                return View(model);
            }

            var response = await _httpClient.PostAsync($"{baseUrl}/api/auth/login", content);
            
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                ViewBag.ErrorMessage = $"API endpoint not found at {baseUrl}/api/auth/login. Please check if the API is running and the URL is correct.";
                return View(model);
            }
            
            if (response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
            {
                ViewBag.ErrorMessage = "API service is unavailable. Please check if the API is running.";
                return View(model);
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            
            System.Diagnostics.Debug.WriteLine($"API Response Content: {responseContent}");
            
            if (string.IsNullOrWhiteSpace(responseContent))
            {
                ViewBag.ErrorMessage = "API returned an empty response. Please check if the API is running and accessible.";
                return View(model);
            }
            
            JsonElement authResponse;
            try
            {
                authResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);
            }
            catch (JsonException ex)
            {
                ViewBag.ErrorMessage = $"Invalid response from API: {ex.Message}. Response content: {responseContent}";
                return View(model);
            }

            if (response.IsSuccessStatusCode)
            {
                if (authResponse.TryGetProperty("success", out var success) && success.GetBoolean())
                {
                    bool isAdmin = false;
                    if (authResponse.TryGetProperty("user", out var userElement) && userElement.ValueKind != JsonValueKind.Null)
                    {
                        if (userElement.TryGetProperty("role", out var role))
                        {
                            if (role.ValueKind == JsonValueKind.Number)
                            {
                                isAdmin = role.GetInt32() == 0; 
                            }
                            else if (role.ValueKind == JsonValueKind.String)
                            {
                                isAdmin = role.GetString() == "0";
                            }
                        }
                    }

                    if (!isAdmin)
                    {
                        ViewBag.ErrorMessage = "Access denied. This application requires Admin privileges. Your account does not have the required role.";
                        return View(model);
                    }

                    if (authResponse.TryGetProperty("accessToken", out var accessToken))
                    {
                        string token = accessToken.ValueKind == JsonValueKind.String 
                            ? accessToken.GetString() ?? "" 
                            : accessToken.ToString();
                        
                        System.Diagnostics.Debug.WriteLine($"Extracted accessToken: {token}, Length: {token?.Length ?? 0}");
                        
                        if (!string.IsNullOrEmpty(token))
                        {
                            HttpContext.Session.SetString("AccessToken", token);
                            System.Diagnostics.Debug.WriteLine($"Setting Session.AccessToken: {token}");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("AccessToken is null or empty after extraction");
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Could not find accessToken property in response");
                    }
                    
                    if (authResponse.TryGetProperty("refreshToken", out var refreshToken))
                    {
                        string refresh = refreshToken.ValueKind == JsonValueKind.String 
                            ? refreshToken.GetString() ?? "" 
                            : refreshToken.ToString();
                        
                        System.Diagnostics.Debug.WriteLine($"Extracted refreshToken: {refresh}, Length: {refresh?.Length ?? 0}");
                        
                        if (!string.IsNullOrEmpty(refresh))
                        {
                            HttpContext.Session.SetString("RefreshToken", refresh);
                            System.Diagnostics.Debug.WriteLine($"Setting Session.RefreshToken: {refresh}");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("RefreshToken is null or empty after extraction");
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Could not find refreshToken property in response");
                    }
                    
                    if (authResponse.TryGetProperty("user", out var userDataElement) && userDataElement.ValueKind != JsonValueKind.Null)
                    {
                        if (userDataElement.TryGetProperty("id", out var id))
                        {
                            string userId = id.ValueKind == JsonValueKind.String 
                                ? id.GetString() ?? "" 
                                : id.ToString();
                            if (!string.IsNullOrEmpty(userId))
                            {
                                HttpContext.Session.SetString("UserId", userId);
                            }
                        }
                        
                        if (userDataElement.TryGetProperty("name", out var name))
                        {
                            string nameValue = name.ValueKind == JsonValueKind.String 
                                ? name.GetString() ?? "" 
                                : name.ToString();
                            
                            if (userDataElement.TryGetProperty("surname", out var surname))
                            {
                                string surnameValue = surname.ValueKind == JsonValueKind.String 
                                    ? surname.GetString() ?? "" 
                                    : surname.ToString();
                                
                                string userName = $"{nameValue} {surnameValue}".Trim();
                                if (!string.IsNullOrEmpty(userName))
                                {
                                    HttpContext.Session.SetString("UserName", userName);
                                }
                            }
                            else if (!string.IsNullOrEmpty(nameValue))
                            {
                                HttpContext.Session.SetString("UserName", nameValue);
                            }
                        }
                        
                        if (userDataElement.TryGetProperty("email", out var email))
                        {
                            string emailValue = email.ValueKind == JsonValueKind.String 
                                ? email.GetString() ?? "" 
                                : email.ToString();
                            if (!string.IsNullOrEmpty(emailValue))
                            {
                                HttpContext.Session.SetString("UserEmail", emailValue);
                            }
                        }
                        
                        if (userDataElement.TryGetProperty("role", out var role))
                        {
                            string roleValue = role.ValueKind == JsonValueKind.String 
                                ? role.GetString() ?? "" 
                                : role.ToString();
                            if (!string.IsNullOrEmpty(roleValue))
                            {
                                HttpContext.Session.SetString("UserRole", roleValue);
                                System.Diagnostics.Debug.WriteLine($"Setting Session.UserRole: {roleValue}");
                            }
                        }
                    }

                    System.Diagnostics.Debug.WriteLine($"Login successful - UserRole: {HttpContext.Session.GetString("UserRole")}");
                    System.Diagnostics.Debug.WriteLine($"Login successful - AccessToken: {HttpContext.Session.GetString("AccessToken")}");

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    var errorMessage = "Login failed. Please check your credentials.";
                    if (authResponse.TryGetProperty("message", out var message))
                    {
                        errorMessage = message.GetString() ?? errorMessage;
                    }
                    
                    System.Diagnostics.Debug.WriteLine($"Login failed - Success=false, Message: {errorMessage}");
                    
                    ViewBag.ErrorMessage = errorMessage;
                    return View(model);
                }
            }
            else
            {
                var errorMessage = "Login failed. Please check your credentials.";
                if (authResponse.TryGetProperty("message", out var message))
                {
                    errorMessage = message.GetString() ?? errorMessage;
                }
                
                System.Diagnostics.Debug.WriteLine($"Login failed - Status: {response.StatusCode}, Message: {errorMessage}");
                
                ViewBag.ErrorMessage = errorMessage;
                return View(model);
            }
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
                
                System.Diagnostics.Debug.WriteLine($"Logout - Using BASE_URL: {baseUrl}");
                System.Diagnostics.Debug.WriteLine($"Logout - Environment: {_configuration["ASPNETCORE_ENVIRONMENT"]}");
                
                var json = JsonSerializer.Serialize(refreshToken);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                await _httpClient.PostAsync($"{baseUrl}/api/auth/logout", content);
            }

            // Clear all session data
            HttpContext.Session.Clear();
            
            System.Diagnostics.Debug.WriteLine("Session cleared during logout");
            
            // Redirect to login page
            return RedirectToAction("Login");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error during logout: {ex.Message}");
            
            // Even if there's an error, clear the session
            HttpContext.Session.Clear();
            
            return RedirectToAction("Login");
        }
    }
}
