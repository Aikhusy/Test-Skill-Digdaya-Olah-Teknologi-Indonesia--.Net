using System.Text.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

public class CustomAuthorizationMiddleware
{
    private readonly RequestDelegate _next;

    public CustomAuthorizationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        await _next(context);

        if (context.User.Identity?.IsAuthenticated == true)
        {
            var userRole = context.User.FindFirst(ClaimTypes.Role)?.Value;

            var endpoint = context.GetEndpoint();
            var metadata = endpoint?.Metadata.GetMetadata<AuthorizeAttribute>();

            if (metadata != null)
            {
                var requiredRoles = metadata.Roles?.Split(',');

                if (requiredRoles != null && !requiredRoles.Contains(userRole))
                {
                    context.Response.StatusCode = 404;
                    context.Response.ContentType = "application/json";
                    var result = JsonSerializer.Serialize(new { status = 404, message = "Not Found" });
                    await context.Response.WriteAsync(result);
                    return;
                }
            }
        }

        if (context.Response.StatusCode == StatusCodes.Status401Unauthorized || 
            context.Response.StatusCode == StatusCodes.Status403Forbidden)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            context.Response.ContentType = "application/json";

            var response = new
            {
                status = 404,
                message = "Not Found"
            };

            var json = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(json);
        }
    }
}
