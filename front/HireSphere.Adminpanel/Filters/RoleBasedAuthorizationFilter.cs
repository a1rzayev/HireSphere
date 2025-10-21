using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HireSphere.Adminpanel.Filters;

public class RoleBasedAuthorizationFilter : IAuthorizationFilter
{
    private readonly int[] _allowedRoles;

    public RoleBasedAuthorizationFilter(params int[] allowedRoles)
    {
        _allowedRoles = allowedRoles;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var accessToken = context.HttpContext.Session.GetString("AccessToken");
        var userRole = context.HttpContext.Session.GetString("UserRole");
        var rememberMe = context.HttpContext.Session.GetInt32("RememberMe");

        // Check if user is authenticated
        if (string.IsNullOrEmpty(accessToken))
        {
            context.Result = new RedirectToActionResult("Login", "Auth", null);
            return;
        }

        // Check if user has required role
        if (string.IsNullOrEmpty(userRole) || !int.TryParse(userRole, out var role))
        {
            context.Result = new RedirectToActionResult("AccessDenied", "Auth", null);
            return;
        }

        // Check if user role is in allowed roles
        if (!_allowedRoles.Contains(role))
        {
            context.Result = new RedirectToActionResult("AccessDenied", "Auth", null);
            return;
        }

        // Refresh Remember Me session if enabled
        if (rememberMe == 1)
        {
            // Extend session for Remember Me users
            context.HttpContext.Session.SetInt32("RememberMe", 1);
        }
    }
}

// Specific role filters for convenience
public class AdminOnlyFilter : RoleBasedAuthorizationFilter
{
    public AdminOnlyFilter() : base(0) { }
}

public class EmployerOnlyFilter : RoleBasedAuthorizationFilter
{
    public EmployerOnlyFilter() : base(1) { }
}

public class JobSeekerOnlyFilter : RoleBasedAuthorizationFilter
{
    public JobSeekerOnlyFilter() : base(2) { }
}

public class EmployerOrAdminFilter : RoleBasedAuthorizationFilter
{
    public EmployerOrAdminFilter() : base(0, 1) { }
}

public class JobSeekerOrAdminFilter : RoleBasedAuthorizationFilter
{
    public JobSeekerOrAdminFilter() : base(0, 2) { }
}
