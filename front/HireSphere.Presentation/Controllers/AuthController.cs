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
            
            System.Diagnostics.Debug.WriteLine($"Using BASE_URL: {baseUrl}");
            System.Diagnostics.Debug.WriteLine($"Environment: {_configuration["ASPNETCORE_ENVIRONMENT"]}");
            
            if (string.IsNullOrEmpty(baseUrl) || baseUrl == "baseurl")
            {
                ViewBag.ErrorMessage = "Configuration error: BASE_URL is not properly configured. Please set BASE_URL in appsettings.json";
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
                Password = model.Password
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
                    if (authResponse.TryGetProperty("accessToken", out var accessToken))
                    {
                        string token = accessToken.ValueKind == JsonValueKind.String 
                            ? accessToken.GetString() ?? "" 
                            : accessToken.ToString();
                        
                        if (!string.IsNullOrEmpty(token))
                        {
                            HttpContext.Session.SetString("AccessToken", token);
                            TempData["AccessToken"] = token;
                            System.Diagnostics.Debug.WriteLine($"Setting TempData.AccessToken: {token}");
                        }
                    }
                    
                    if (authResponse.TryGetProperty("refreshToken", out var refreshToken))
                    {
                        string refresh = refreshToken.ValueKind == JsonValueKind.String 
                            ? refreshToken.GetString() ?? "" 
                            : refreshToken.ToString();
                        
                        if (!string.IsNullOrEmpty(refresh))
                        {
                            HttpContext.Session.SetString("RefreshToken", refresh);
                            TempData["RefreshToken"] = refresh;
                            System.Diagnostics.Debug.WriteLine($"Setting TempData.RefreshToken: {refresh}");
                        }
                    }
                    
                    if (authResponse.TryGetProperty("user", out var userElement) && userElement.ValueKind != JsonValueKind.Null)
                    {
                        if (userElement.TryGetProperty("id", out var id))
                        {
                            string userId = id.ValueKind == JsonValueKind.String 
                                ? id.GetString() ?? "" 
                                : id.ToString();
                            if (!string.IsNullOrEmpty(userId))
                            {
                                HttpContext.Session.SetString("UserId", userId);

                            }
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
                        
                        if (userElement.TryGetProperty("email", out var email))
                        {
                            string emailValue = email.ValueKind == JsonValueKind.String 
                                ? email.GetString() ?? "" 
                                : email.ToString();
                            if (!string.IsNullOrEmpty(emailValue))
                            {
                                HttpContext.Session.SetString("UserEmail", emailValue);

                            }
                        }
                        
                        if (userElement.TryGetProperty("role", out var role))
                        {
                            string roleValue = role.ValueKind == JsonValueKind.String 
                                ? role.GetString() ?? "" 
                                : role.ToString();
                            if (!string.IsNullOrEmpty(roleValue))
                            {
                                HttpContext.Session.SetString("UserRole", roleValue);

                            }
                        }
                    }

                    System.Diagnostics.Debug.WriteLine($"Before redirect - TempData.AccessToken: {TempData["AccessToken"]}");
                    System.Diagnostics.Debug.WriteLine($"Before redirect - TempData.RefreshToken: {TempData["RefreshToken"]}");

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

            HttpContext.Session.Clear();
            
            ViewBag.ClearLocalStorage = true;
        }
        catch (Exception)
        {
            //Logging in the future
            ViewBag.ClearLocalStorage = true;
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Profile()
    {
        var accessToken = HttpContext.Session.GetString("AccessToken");
        if (string.IsNullOrEmpty(accessToken))
        {
            return RedirectToAction("Login");
        }

        var profile = new ProfileViewModel
        {
            Name = HttpContext.Session.GetString("UserName")?.Split(' ')[0] ?? "",
            Surname = HttpContext.Session.GetString("UserName")?.Split(' ').Length > 1 ? string.Join(" ", HttpContext.Session.GetString("UserName")?.Split(' ').Skip(1) ?? new string[0]) : "",
            Email = HttpContext.Session.GetString("UserEmail") ?? ""
        };

        return View(profile);
    }

    [HttpPost]
    public async Task<IActionResult> Profile(ProfileViewModel model)
    {
        try
        {
            var accessToken = HttpContext.Session.GetString("AccessToken");
            if (string.IsNullOrEmpty(accessToken))
            {
                return RedirectToAction("Login");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var baseUrl = _configuration["BASE_URL"];
            
            if (string.IsNullOrEmpty(baseUrl) || baseUrl == "baseurl")
            {
                ViewBag.ErrorMessage = "Configuration error: BASE_URL is not properly configured. Please set BASE_URL in appsettings.json";
                return View(model);
            }

            var updateRequest = new
            {
                Name = model.Name.Trim(),
                Surname = model.Surname.Trim(),
                Email = model.Email.Trim(),
                PhoneNumber = model.PhoneNumber?.Trim(),
                Bio = model.Bio?.Trim(),
                Location = model.Location?.Trim(),
                Website = model.Website?.Trim(),
                Company = model.Company?.Trim()
            };

            var json = JsonSerializer.Serialize(updateRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Put, $"{baseUrl}/api/user/profile");
            request.Content = content;
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                HttpContext.Session.SetString("UserName", $"{model.Name} {model.Surname}".Trim());
                HttpContext.Session.SetString("UserEmail", model.Email);

                ViewBag.SuccessMessage = "Profile updated successfully!";
                return View(model);
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            ViewBag.ErrorMessage = $"Failed to update profile: {errorContent}";
            return View(model);
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = $"An error occurred while updating profile: {ex.Message}";
            return View(model);
        }
    }
}
