using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HireSphere.Adminpanel.Models;
using Microsoft.AspNetCore.Authorization;
using HireSphere.Adminpanel.Filters;

namespace HireSphere.Adminpanel.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        var accessToken = HttpContext.Session.GetString("AccessToken");
        var userRole = HttpContext.Session.GetString("UserRole");

        // Check if user is authenticated
        if (string.IsNullOrEmpty(accessToken))
        {
            return RedirectToAction("Login", "Auth");
        }

        // Redirect based on user role
        if (int.TryParse(userRole, out var role))
        {
            return role switch
            {
                0 => View(), // Admin - show admin dashboard
                1 => RedirectToAction("Dashboard", "Employer"), // Employer
                2 => RedirectToAction("Dashboard", "JobSeeker"), // JobSeeker
                _ => RedirectToAction("AccessDenied", "Auth")
            };
        }

        return RedirectToAction("AccessDenied", "Auth");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
