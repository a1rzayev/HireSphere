using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HireSphere.Adminpanel.Filters;

public class SessionAuthorizationFilter : IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var accessToken = context.HttpContext.Session.GetString("AccessToken");
        var userRole = context.HttpContext.Session.GetString("UserRole");

        // Check if user is authenticated
        if (string.IsNullOrEmpty(accessToken))
        {
            context.Result = new RedirectToActionResult("Login", "Auth", null);
            return;
        }

        // Check if user has admin role (Role = 0)
        if (string.IsNullOrEmpty(userRole) || !int.TryParse(userRole, out var role) || role != 0)
        {
            context.Result = new RedirectToActionResult("Login", "Auth", null);
            return;
        }
    }
}
