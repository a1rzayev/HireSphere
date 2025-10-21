using System;
using System.ComponentModel.DataAnnotations;

namespace HireSphere.Presentation.Models;

public class PostViewModel
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public string AuthorEmail { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int LikesCount { get; set; }
    public int CommentsCount { get; set; }
    public List<string> Tags { get; set; } = new List<string>();
    public string? ImageUrl { get; set; }
    public bool IsLiked { get; set; }
    public bool IsAuthor { get; set; }

    public string CreatedDate => CreatedAt.ToString("MMM dd, yyyy");
    public string CreatedTime => CreatedAt.ToString("h:mm tt");
    public string TimeAgo => GetTimeAgo(CreatedAt);
    public string ShortContent => Content.Length > 200 ? Content.Substring(0, 200) + "..." : Content;
    public bool HasImage => !string.IsNullOrEmpty(ImageUrl);

    private string GetTimeAgo(DateTime dateTime)
    {
        var timeSpan = DateTime.Now - dateTime;
        
        if (timeSpan.TotalDays >= 1)
        {
            var days = (int)timeSpan.TotalDays;
            return days == 1 ? "1 day ago" : $"{days} days ago";
        }
        else if (timeSpan.TotalHours >= 1)
        {
            var hours = (int)timeSpan.TotalHours;
            return hours == 1 ? "1 hour ago" : $"{hours} hours ago";
        }
        else if (timeSpan.TotalMinutes >= 1)
        {
            var minutes = (int)timeSpan.TotalMinutes;
            return minutes == 1 ? "1 minute ago" : $"{minutes} minutes ago";
        }
        else
        {
            return "Just now";
        }
    }
}

public class CreatePostViewModel
{
    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, MinimumLength = 5, ErrorMessage = "Title must be between 5 and 200 characters")]
    [Display(Name = "Post Title")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Content is required")]
    [StringLength(2000, MinimumLength = 10, ErrorMessage = "Content must be between 10 and 2000 characters")]
    [Display(Name = "What's on your mind?")]
    public string Content { get; set; } = string.Empty;

    [Display(Name = "Tags (comma-separated)")]
    [StringLength(500, ErrorMessage = "Tags cannot exceed 500 characters")]
    public string? Tags { get; set; }

    [Display(Name = "Image URL")]
    [Url(ErrorMessage = "Please enter a valid URL")]
    public string? ImageUrl { get; set; }
}

public class PostsListViewModel
{
    public List<PostViewModel> Posts { get; set; } = new();
    public int TotalCount { get; set; }
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalPages { get; set; } = 1;
    public string? SearchTerm { get; set; }
    public string? TagFilter { get; set; }

    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;
    public int PreviousPage => Math.Max(1, CurrentPage - 1);
    public int NextPage => Math.Min(TotalPages, CurrentPage + 1);
}

public class HomePageViewModel
{
    public List<PostViewModel> RecentPosts { get; set; } = new();
    public List<PostViewModel> FeaturedPosts { get; set; } = new();
    public int TotalPosts { get; set; }
    public int TotalUsers { get; set; }
    public List<string> PopularTags { get; set; } = new();
}
