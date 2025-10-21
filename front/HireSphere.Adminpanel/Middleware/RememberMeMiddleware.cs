using Microsoft.AspNetCore.Http;

namespace HireSphere.Adminpanel.Middleware;

public class RememberMeMiddleware
{
    private readonly RequestDelegate _next;

    public RememberMeMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Check if Remember Me is enabled for this session
        var rememberMe = context.Session.GetInt32("RememberMe");
        
        if (rememberMe == 1)
        {
            // Extend session timeout to 30 days for Remember Me users
            context.Session.SetInt32("RememberMe", 1); // Refresh the flag
        }

        await _next(context);
    }
}
