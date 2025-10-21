using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HireSphere.Presentation.Models;
using System.Text.Json;

namespace HireSphere.Presentation.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public HomeController(ILogger<HomeController> logger, IConfiguration configuration, HttpClient httpClient)
    {
        _logger = logger;
        _configuration = configuration;
        _httpClient = httpClient;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var baseUrl = _configuration["BASE_URL"];
            if (string.IsNullOrEmpty(baseUrl) || baseUrl == "baseurl")
            {
                ViewBag.ErrorMessage = "Configuration error: BASE_URL is not properly configured.";
                return View(new HomePageViewModel());
            }

            // Fetch recent posts
            var postsResponse = await _httpClient.GetAsync($"{baseUrl}/api/posts?page=1&pageSize=5");
            var recentPosts = new List<PostViewModel>();

            if (postsResponse.IsSuccessStatusCode)
            {
                var postsContent = await postsResponse.Content.ReadAsStringAsync();
                var postsData = JsonSerializer.Deserialize<JsonElement>(postsContent);

                if (postsData.TryGetProperty("posts", out var postsArray))
                {
                    foreach (var post in postsArray.EnumerateArray())
                    {
                        try
                        {
                            var tags = new List<string>();
                            if (post.TryGetProperty("tags", out var tagsArray))
                            {
                                foreach (var tagItem in tagsArray.EnumerateArray())
                                {
                                    tags.Add(tagItem.GetString() ?? "");
                                }
                            }

                            recentPosts.Add(new PostViewModel
                            {
                                Id = post.TryGetProperty("id", out var id) ? Guid.Parse(id.GetString() ?? "") : Guid.Empty,
                                Title = post.TryGetProperty("title", out var title) ? title.GetString() ?? "" : "",
                                Content = post.TryGetProperty("content", out var content) ? content.GetString() ?? "" : "",
                                AuthorName = post.TryGetProperty("authorName", out var authorName) ? authorName.GetString() ?? "" : "",
                                AuthorEmail = post.TryGetProperty("authorEmail", out var authorEmail) ? authorEmail.GetString() ?? "" : "",
                                CreatedAt = post.TryGetProperty("createdAt", out var createdAt) ? DateTime.Parse(createdAt.GetString() ?? "") : DateTime.Now,
                                UpdatedAt = post.TryGetProperty("updatedAt", out var updatedAt) ? DateTime.Parse(updatedAt.GetString() ?? "") : null,
                                LikesCount = post.TryGetProperty("likesCount", out var likesCount) ? likesCount.GetInt32() : 0,
                                CommentsCount = post.TryGetProperty("commentsCount", out var commentsCount) ? commentsCount.GetInt32() : 0,
                                Tags = tags,
                                ImageUrl = post.TryGetProperty("imageUrl", out var imageUrl) ? imageUrl.GetString() : null,
                                IsLiked = post.TryGetProperty("isLiked", out var isLiked) ? isLiked.GetBoolean() : false,
                                IsAuthor = post.TryGetProperty("isAuthor", out var isAuthor) ? isAuthor.GetBoolean() : false
                            });
                        }
                        catch (Exception postEx)
                        {
                            _logger.LogError(postEx, "Error parsing individual post");
                        }
                    }
                }
            }

            // Fetch home page data from API
            var homeResponse = await _httpClient.GetAsync($"{baseUrl}/api/home");
            var homePageData = new HomePageViewModel
            {
                RecentPosts = recentPosts,
                FeaturedPosts = new List<PostViewModel>(), // Could be populated from API
                TotalPosts = recentPosts.Count,
                TotalUsers = 0, // Could be fetched from API
                PopularTags = recentPosts.SelectMany(p => p.Tags).Distinct().Take(10).ToList()
            };

            if (homeResponse.IsSuccessStatusCode)
            {
                var homeContent = await homeResponse.Content.ReadAsStringAsync();
                var homeData = JsonSerializer.Deserialize<JsonElement>(homeContent);

                if (homeData.TryGetProperty("totalJobs", out var totalJobs))
                {
                    // You could add job-related data here if needed
                }
            }

            return View(homePageData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading home page data");
            return View(new HomePageViewModel());
        }
    }


    public IActionResult Debug()
    {
        var config = new
        {
            Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
            BaseUrl = HttpContext.RequestServices.GetService<IConfiguration>()?["BASE_URL"],
            IsDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development"
        };
        
        return Json(config);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
