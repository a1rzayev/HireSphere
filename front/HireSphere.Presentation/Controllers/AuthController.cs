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
                ConfirmPassword = model.ConfirmPassword,
                Role = 2
            };

            var json = JsonSerializer.Serialize(registerRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{baseUrl}/api/auth/register", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var authResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);
                
                if (authResponse.TryGetProperty("success", out var success) && success.GetBoolean())
                {
                    ViewBag.SuccessMessage = "Registration successful! Please log in with your new account.";
                    return RedirectToAction("Login");
                }
                else
                {
                    // Handle API error response
                    if (authResponse.TryGetProperty("message", out var message))
                    {
                        var errorMessage = message.GetString() ?? "Registration failed. Please try again.";
                        
                        // Convert technical error messages to user-friendly ones
                        if (errorMessage.Contains("already exists") || errorMessage.Contains("email already"))
                        {
                            ViewBag.ErrorMessage = "This email is already in use. Please use a different email address.";
                        }
                        else if (errorMessage.Contains("password"))
                        {
                            ViewBag.ErrorMessage = "Password requirements not met. Please check your password.";
                        }
                        else if (errorMessage.Contains("validation"))
                        {
                            ViewBag.ErrorMessage = "Please check all fields and try again.";
                        }
                        else
                        {
                            ViewBag.ErrorMessage = errorMessage;
                        }
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Registration failed. Please try again.";
                    }
                    return View(model);
                }
            }
            else
            {
                // Handle HTTP error response
                var errorContent = await response.Content.ReadAsStringAsync();
                try
                {
                    var errorResponse = JsonSerializer.Deserialize<JsonElement>(errorContent);
                    if (errorResponse.TryGetProperty("message", out var errorMessage))
                    {
                        var apiErrorMessage = errorMessage.GetString() ?? "Registration failed. Please try again.";
                        
                        // Convert technical error messages to user-friendly ones
                        if (apiErrorMessage.Contains("already exists") || apiErrorMessage.Contains("email already"))
                        {
                            ViewBag.ErrorMessage = "This email is already in use. Please use a different email address.";
                        }
                        else if (apiErrorMessage.Contains("password"))
                        {
                            ViewBag.ErrorMessage = "Password requirements not met. Please check your password.";
                        }
                        else if (apiErrorMessage.Contains("validation"))
                        {
                            ViewBag.ErrorMessage = "Please check all fields and try again.";
                        }
                        else
                        {
                            ViewBag.ErrorMessage = apiErrorMessage;
                        }
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Registration failed. Please try again.";
                    }
                }
                catch
                {
                    ViewBag.ErrorMessage = "Registration failed. Please try again.";
                }
                return View(model);
            }
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
                Password = model.Password,
                Role = 2
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
                    bool hasValidRole = false;
                    if (authResponse.TryGetProperty("user", out var userElement) && userElement.ValueKind != JsonValueKind.Null)
                    {
                        if (userElement.TryGetProperty("role", out var role))
                        {
                            int userRole = 0;
                            if (role.ValueKind == JsonValueKind.Number)
                            {
                                userRole = role.GetInt32();
                            }
                            else if (role.ValueKind == JsonValueKind.String)
                            {
                                int.TryParse(role.GetString(), out userRole);
                            }
                            
                            hasValidRole = userRole == 1 || userRole == 2;
                        }
                    }

                    if (!hasValidRole)
                    {
                        ViewBag.ErrorMessage = "Access denied. This application is for Job Seekers and Employers. Your account has Admin privileges. Please use the Admin Panel application instead.";
                        return View(model);
                    }

                    if (authResponse.TryGetProperty("accessToken", out var accessToken))
                    {
                        string token = accessToken.ValueKind == JsonValueKind.String 
                            ? accessToken.GetString() ?? "" 
                            : accessToken.ToString();
                        
                        if (!string.IsNullOrEmpty(token))
                        {
                            HttpContext.Session.SetString("AccessToken", token);
                            System.Diagnostics.Debug.WriteLine($"Setting Session.AccessToken: {token}");
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
                            System.Diagnostics.Debug.WriteLine($"Setting Session.RefreshToken: {refresh}");
                        }
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
                            }
                        }
                    }

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    var errorMessage = "Login failed. Please check your credentials.";
                    if (authResponse.TryGetProperty("message", out var message))
                    {
                        errorMessage = message.GetString() ?? errorMessage;
                    }
                    
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
            ViewBag.ClearLocalStorage = true;
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        var accessToken = HttpContext.Session.GetString("AccessToken");
        if (string.IsNullOrEmpty(accessToken))
        {
            return RedirectToAction("Login");
        }

        var profile = new ProfileViewModel
        {
            Name = HttpContext.Session.GetString("UserName")?.Split(' ')[0],
            Surname = HttpContext.Session.GetString("UserName")?.Split(' ').Length > 1 ? string.Join(" ", HttpContext.Session.GetString("UserName")?.Split(' ').Skip(1) ?? new string[0]) : null,
            Email = HttpContext.Session.GetString("UserEmail"),
            PhoneNumber = HttpContext.Session.GetString("UserPhone"),
            Company = HttpContext.Session.GetString("UserCompany")
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

            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                ViewBag.ErrorMessage = "User ID not found in session. Please log in again.";
                return View(model);
            }

            var getUserRequest = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}/api/user/{userId}");
            getUserRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            
            var getUserResponse = await _httpClient.SendAsync(getUserRequest);
            if (!getUserResponse.IsSuccessStatusCode)
            {
                ViewBag.ErrorMessage = "Failed to fetch current user data. Please try again.";
                return View(model);
            }

            var currentUserJson = await getUserResponse.Content.ReadAsStringAsync();
            var currentUser = JsonSerializer.Deserialize<JsonElement>(currentUserJson);

            var currentRoleString = GetJsonValueAsString(currentUser, "role");
            var currentRole = ParseRole(currentRoleString);
            
            var updateRequest = new
            {
                name = !string.IsNullOrWhiteSpace(model.Name) ? model.Name.Trim() : "",
                surname = !string.IsNullOrWhiteSpace(model.Surname) ? model.Surname.Trim() : "",
                email = !string.IsNullOrWhiteSpace(model.Email) ? model.Email.Trim() : "",
                phone = !string.IsNullOrWhiteSpace(model.PhoneNumber) ? model.PhoneNumber.Trim() : null,
                passwordHash = GetJsonValueAsString(currentUser, "passwordHash"),
                role = currentRole
            };

            var json = JsonSerializer.Serialize(updateRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Put, $"{baseUrl}/api/user/{userId}");
            request.Content = content;
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                await RefreshUserSessionAsync(userId, accessToken, baseUrl);

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

    private string GetJsonValueAsString(JsonElement element, string propertyName)
    {
        try
        {
            if (element.TryGetProperty(propertyName, out var property))
            {
                return property.ValueKind switch
                {
                    JsonValueKind.String => property.GetString() ?? "",
                    JsonValueKind.Number => property.GetInt64().ToString(),
                    JsonValueKind.True => "true",
                    JsonValueKind.False => "false",
                    JsonValueKind.Null => "",
                    _ => property.ToString()
                };
            }
            return "";
        }
        catch
        {
            return "";
        }
    }

    private int ParseRole(string roleString)
    {
        if (string.IsNullOrWhiteSpace(roleString))
            return 2;
            
        roleString = roleString.Trim().Trim('"', '\'');
        
        if (int.TryParse(roleString, out var roleNumber))
        {
            return roleNumber switch
            {
                0 => 0, 
                1 => 1, 
                2 => 2, 
                _ => 2  
            };
        }
        
        if (roleString.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            return 0;
        if (roleString.Equals("Employer", StringComparison.OrdinalIgnoreCase))
            return 1;
        if (roleString.Equals("JobSeeker", StringComparison.OrdinalIgnoreCase))
            return 2;
        
        return 2; 
    }

    private async Task RefreshUserSessionAsync(string userId, string accessToken, string baseUrl)
    {
        try
        {
            var refreshUserRequest = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}/api/user/{userId}");
            refreshUserRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            
            var refreshUserResponse = await _httpClient.SendAsync(refreshUserRequest);
            if (refreshUserResponse.IsSuccessStatusCode)
            {
                var refreshUserJson = await refreshUserResponse.Content.ReadAsStringAsync();
                var refreshUser = JsonSerializer.Deserialize<JsonElement>(refreshUserJson);
                
                var updatedName = GetJsonValueAsString(refreshUser, "name");
                var updatedSurname = GetJsonValueAsString(refreshUser, "surname");
                var updatedEmail = GetJsonValueAsString(refreshUser, "email");
                var updatedRole = GetJsonValueAsString(refreshUser, "role");
                var updatedPhone = GetJsonValueAsString(refreshUser, "phone");
                var updatedCompany = GetJsonValueAsString(refreshUser, "company");
                
                if (!string.IsNullOrWhiteSpace(updatedName) || !string.IsNullOrWhiteSpace(updatedSurname))
                {
                    var fullName = $"{updatedName} {updatedSurname}".Trim();
                    if (!string.IsNullOrWhiteSpace(fullName))
                    {
                        HttpContext.Session.SetString("UserName", fullName);
                    }
                }
                
                if (!string.IsNullOrWhiteSpace(updatedEmail))
                {
                    HttpContext.Session.SetString("UserEmail", updatedEmail);
                }
                
                if (!string.IsNullOrWhiteSpace(updatedRole))
                {
                    HttpContext.Session.SetString("UserRole", updatedRole);
                }
                
                if (!string.IsNullOrWhiteSpace(updatedPhone))
                {
                    HttpContext.Session.SetString("UserPhone", updatedPhone);
                }
                
                if (!string.IsNullOrWhiteSpace(updatedCompany))
                {
                    HttpContext.Session.SetString("UserCompany", updatedCompany);
                }
            }
        }
        catch (Exception)
        {
            // Silently fail if refresh fails
        }
    }
}
