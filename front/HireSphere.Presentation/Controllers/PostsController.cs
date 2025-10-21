using Microsoft.AspNetCore.Mvc;
using HireSphere.Presentation.Models;
using System.Text;
using System.Text.Json;

namespace HireSphere.Presentation.Controllers;

public class PostsController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private readonly ILogger<PostsController> _logger;

    public PostsController(IConfiguration configuration, HttpClient httpClient, ILogger<PostsController> logger)
    {
        _configuration = configuration;
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<IActionResult> Index(string? search, string? tag, int page = 1, int pageSize = 10)
    {
        try
        {
            var baseUrl = _configuration["BASE_URL"];
            if (string.IsNullOrEmpty(baseUrl) || baseUrl == "baseurl")
            {
                ViewBag.ErrorMessage = "Configuration error: BASE_URL is not properly configured.";
                return View(new PostsListViewModel());
            }

            var queryParams = new List<string>();
            if (!string.IsNullOrWhiteSpace(search))
                queryParams.Add($"search={Uri.EscapeDataString(search)}");
            if (!string.IsNullOrWhiteSpace(tag))
                queryParams.Add($"tag={Uri.EscapeDataString(tag)}");
            if (page > 1)
                queryParams.Add($"page={page}");
            if (pageSize != 10)
                queryParams.Add($"pageSize={pageSize}");

            var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
            var url = $"{baseUrl}/api/posts{queryString}";

            _logger.LogInformation($"Making request to: {url}");
            var response = await _httpClient.GetAsync(url);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"API Response: {content}");
                
                try
                {
                    var postsData = JsonSerializer.Deserialize<JsonElement>(content);

                    var posts = new List<PostViewModel>();
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

                                posts.Add(new PostViewModel
                                {
                                    Id = post.TryGetProperty("id", out var id) ? Guid.Parse(id.GetString() ?? "") : Guid.Empty,
                                    Title = post.TryGetProperty("title", out var title) ? title.GetString() ?? "" : "",
                                    Content = post.TryGetProperty("content", out var contentProp) ? contentProp.GetString() ?? "" : "",
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

                    var totalCount = postsData.TryGetProperty("totalCount", out var total) ? total.GetInt32() : 0;
                    var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                    var viewModel = new PostsListViewModel
                    {
                        Posts = posts,
                        SearchTerm = search ?? "",
                        TagFilter = tag ?? "",
                        CurrentPage = page,
                        PageSize = pageSize,
                        TotalPages = totalPages,
                        TotalCount = totalCount
                    };

                    _logger.LogInformation($"Successfully loaded {posts.Count} posts");
                    return View(viewModel);
                }
                catch (Exception parseEx)
                {
                    _logger.LogError(parseEx, "Error parsing API response");
                    ViewBag.ErrorMessage = "Error parsing posts data. Please try again later.";
                    return View(new PostsListViewModel());
                }
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError($"API request failed with status {response.StatusCode}: {errorContent}");
                ViewBag.ErrorMessage = $"Failed to load posts. API returned status {response.StatusCode}. Please try again later.";
                return View(new PostsListViewModel());
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading posts");
            ViewBag.ErrorMessage = "An error occurred while loading posts.";
            return View(new PostsListViewModel());
        }
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreatePostViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreatePostViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var baseUrl = _configuration["BASE_URL"];
            if (string.IsNullOrEmpty(baseUrl) || baseUrl == "baseurl")
            {
                ViewBag.ErrorMessage = "Configuration error: BASE_URL is not properly configured.";
                return View(model);
            }

            var postData = new
            {
                title = model.Title,
                content = model.Content,
                tags = !string.IsNullOrEmpty(model.Tags) ? model.Tags.Split(',').Select(t => t.Trim()).Where(t => !string.IsNullOrEmpty(t)).ToArray() : new string[0],
                imageUrl = model.ImageUrl
            };

            var json = JsonSerializer.Serialize(postData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{baseUrl}/api/posts", content);
            
            if (response.IsSuccessStatusCode)
            {
                ViewBag.SuccessMessage = "Post created successfully!";
                return RedirectToAction("Index");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Failed to create post: {errorContent}");
                ViewBag.ErrorMessage = "Failed to create post. Please try again.";
                return View(model);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating post");
            ViewBag.ErrorMessage = "An error occurred while creating the post.";
            return View(model);
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
                return View(new PostViewModel());
            }

            var response = await _httpClient.GetAsync($"{baseUrl}/api/posts/{id}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var postData = JsonSerializer.Deserialize<JsonElement>(content);

                var tags = new List<string>();
                if (postData.TryGetProperty("tags", out var tagsArray))
                {
                    foreach (var tag in tagsArray.EnumerateArray())
                    {
                        tags.Add(tag.GetString() ?? "");
                    }
                }

                var post = new PostViewModel
                {
                    Id = postData.TryGetProperty("id", out var idProp) ? Guid.Parse(idProp.GetString() ?? "") : Guid.Empty,
                    Title = postData.TryGetProperty("title", out var title) ? title.GetString() ?? "" : "",
                    Content = postData.TryGetProperty("content", out var contentProp) ? contentProp.GetString() ?? "" : "",
                    AuthorName = postData.TryGetProperty("authorName", out var authorName) ? authorName.GetString() ?? "" : "",
                    AuthorEmail = postData.TryGetProperty("authorEmail", out var authorEmail) ? authorEmail.GetString() ?? "" : "",
                    CreatedAt = postData.TryGetProperty("createdAt", out var createdAt) ? DateTime.Parse(createdAt.GetString() ?? "") : DateTime.Now,
                    UpdatedAt = postData.TryGetProperty("updatedAt", out var updatedAt) ? DateTime.Parse(updatedAt.GetString() ?? "") : null,
                    LikesCount = postData.TryGetProperty("likesCount", out var likesCount) ? likesCount.GetInt32() : 0,
                    CommentsCount = postData.TryGetProperty("commentsCount", out var commentsCount) ? commentsCount.GetInt32() : 0,
                    Tags = tags,
                    ImageUrl = postData.TryGetProperty("imageUrl", out var imageUrl) ? imageUrl.GetString() : null,
                    IsLiked = postData.TryGetProperty("isLiked", out var isLiked) ? isLiked.GetBoolean() : false,
                    IsAuthor = postData.TryGetProperty("isAuthor", out var isAuthor) ? isAuthor.GetBoolean() : false
                };

                return View(post);
            }
            else
            {
                ViewBag.ErrorMessage = "Post not found.";
                return View(new PostViewModel());
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading post details");
            ViewBag.ErrorMessage = "An error occurred while loading post details.";
            return View(new PostViewModel());
        }
    }

    public async Task<IActionResult> Search(string? search, string? tag)
    {
        return RedirectToAction("Index", new { search, tag });
    }
}
