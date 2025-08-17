using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using HireSphere.Adminpanel.Models;
using Microsoft.AspNetCore.Authorization;
using HireSphere.Adminpanel.Filters;

namespace HireSphere.Adminpanel.Controllers;

[ServiceFilter(typeof(SessionAuthorizationFilter))]
public class UserController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public UserController(IConfiguration configuration, HttpClient httpClient)
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
                if (!string.IsNullOrEmpty(content))
                {
                    try
                    {
                        var users = JsonSerializer.Deserialize<List<User>>(content, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                        
                        if (users != null)
                        {
                            var userViewModels = users.Select(u => new UserViewModel
                            {
                                Id = u.Id,
                                Name = u.Name,
                                Surname = u.Surname,
                                Email = u.Email,
                                Phone = u.Phone,
                                Role = (int)u.Role,
                                CreatedAt = u.CreatedAt,
                                IsEmailConfirmed = u.IsEmailConfirmed
                            }).ToList();
                            
                            ViewBag.SuccessMessage = $"Successfully loaded {userViewModels.Count} users.";
                            return View(userViewModels);
                        }
                        else
                        {
                            ViewBag.ErrorMessage = "Failed to deserialize users data";
                            return View(new List<UserViewModel>());
                        }
                    }
                    catch (JsonException ex)
                    {
                        ViewBag.ErrorMessage = $"Failed to parse users data: {ex.Message}";
                        return View(new List<UserViewModel>());
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = "API returned empty response";
                    return View(new List<UserViewModel>());
                }
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                ViewBag.ErrorMessage = $"Failed to fetch users: {response.StatusCode} - {errorContent}";
                return View(new List<UserViewModel>());
            }
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = $"An error occurred while fetching users: {ex.Message}";
            return View(new List<UserViewModel>());
        }
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        if (id == Guid.Empty)
        {
            ViewBag.ErrorMessage = "Invalid user ID provided.";
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

            var request = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}/api/user/{id}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(content))
                {
                    var userViewModel = CreateUserViewModelFromJson(content);
                    
                    if (userViewModel != null && userViewModel.Id != Guid.Empty)
                    {
                        return View(userViewModel);
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Failed to parse user data from the API response or user not found.";
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = "API returned empty response for user details.";
                }
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                ViewBag.ErrorMessage = "User not found. The requested user may have been deleted or never existed.";
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
        if (id == Guid.Empty)
        {
            TempData["ErrorMessage"] = "Invalid user ID provided.";
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

            var request = new HttpRequestMessage(HttpMethod.Delete, $"{baseUrl}/api/user/{id}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "User deleted successfully.";
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                TempData["ErrorMessage"] = "User not found. It may have already been deleted.";
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



    private UserDetailsViewModel? CreateUserViewModelFromJson(string jsonContent)
    {
        try
        {
            using var document = JsonDocument.Parse(jsonContent);
            var root = document.RootElement;

            var viewModel = new UserDetailsViewModel
            {
                Id = GetGuidProperty(root, "id"),
                FullName = $"{GetStringProperty(root, "name")} {GetStringProperty(root, "surname")}".Trim(),
                Email = GetStringProperty(root, "email"),
                RoleValue = GetIntProperty(root, "role"),
                Phone = GetStringProperty(root, "phone"),
                CreatedAt = GetDateTimeProperty(root, "createdAt"),
                IsEmailConfirmed = GetBoolProperty(root, "isEmailConfirmed")
            };

            if (viewModel.Id == Guid.Empty)
            {
                return null;
            }

            viewModel.RoleName = viewModel.RoleValue switch
            {
                0 => "Admin",
                1 => "Employer", 
                2 => "Job Seeker",
                _ => "Unknown"
            };

            viewModel.UserId = viewModel.Id.ToString();

            return viewModel;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error parsing user JSON: {ex.Message}");
            return null;
        }
    }

    private Guid GetGuidProperty(JsonElement element, string propertyName)
    {
        try
        {
            if (element.TryGetProperty(propertyName, out var prop))
            {
                if (prop.ValueKind == JsonValueKind.String)
                {
                    var stringValue = prop.GetString();
                    if (Guid.TryParse(stringValue, out var guid))
                        return guid;
                }
                else if (prop.ValueKind == JsonValueKind.Number && prop.TryGetInt64(out var longValue))
                {
                    return new Guid(longValue.ToString());
                }
            }
        }
        catch
        {
            // Ignore parsing errors
        }
        return Guid.Empty;
    }

    private string GetStringProperty(JsonElement element, string propertyName)
    {
        try
        {
            if (element.TryGetProperty(propertyName, out var prop))
            {
                if (prop.ValueKind == JsonValueKind.String)
                {
                    return prop.GetString() ?? string.Empty;
                }
                else if (prop.ValueKind == JsonValueKind.Number)
                {
                    return prop.GetInt32().ToString();
                }
                else if (prop.ValueKind == JsonValueKind.True || prop.ValueKind == JsonValueKind.False)
                {
                    return prop.GetBoolean().ToString();
                }
            }
        }
        catch
        {
            // Ignore parsing errors
        }
        return string.Empty;
    }

    private int GetIntProperty(JsonElement element, string propertyName)
    {
        try
        {
            if (element.TryGetProperty(propertyName, out var prop))
            {
                if (prop.ValueKind == JsonValueKind.Number && prop.TryGetInt32(out var value))
                    return value;
                if (prop.ValueKind == JsonValueKind.String && int.TryParse(prop.GetString(), out var parsedValue))
                    return parsedValue;
            }
        }
        catch
        {
            // Ignore parsing errors
        }
        return 0;
    }

    private DateTime? GetDateTimeProperty(JsonElement element, string propertyName)
    {
        try
        {
            if (element.TryGetProperty(propertyName, out var prop))
            {
                if (prop.ValueKind == JsonValueKind.String)
                {
                    var dateString = prop.GetString();
                    if (DateTime.TryParse(dateString, out var date))
                        return date;
                }
                else if (prop.ValueKind == JsonValueKind.Number)
                {
                    if (prop.TryGetInt64(out var timestamp))
                    {
                        return DateTimeOffset.FromUnixTimeSeconds(timestamp).DateTime;
                    }
                }
            }
        }
        catch
        {
            // Ignore parsing errors
        }
        return null;
    }

    private bool? GetBoolProperty(JsonElement element, string propertyName)
    {
        try
        {
            if (element.TryGetProperty(propertyName, out var prop))
            {
                if (prop.ValueKind == JsonValueKind.True)
                    return true;
                if (prop.ValueKind == JsonValueKind.False)
                    return false;
                if (prop.ValueKind == JsonValueKind.String && bool.TryParse(prop.GetString(), out var parsedValue))
                    return parsedValue;
                if (prop.ValueKind == JsonValueKind.Number && prop.TryGetInt32(out var intValue))
                    return intValue != 0;
            }
        }
        catch
        {
            // Ignore parsing errors
        }
        return null;
    }
}
