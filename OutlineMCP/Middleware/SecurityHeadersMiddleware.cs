using Microsoft.Extensions.Options;

namespace OutlineMCP.Middleware;

/// <summary>
/// Middleware to add security headers to HTTP responses
/// </summary>
public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;
    private readonly SecurityHeadersOptions _options;

    public SecurityHeadersMiddleware(RequestDelegate next, IOptions<SecurityHeadersOptions> options)
    {
        _next = next;
        _options = options.Value;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (_options.EnableSecurityHeaders)
        {
            // Add security headers
            var headers = context.Response.Headers;

            // Prevent MIME type sniffing
            headers.Append("X-Content-Type-Options", "nosniff");

            // Prevent clickjacking
            headers.Append("X-Frame-Options", "DENY");

            // Enable XSS protection
            headers.Append("X-XSS-Protection", "1; mode=block");

            // Referrer policy
            headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");

            // Content Security Policy for API endpoints
            headers.Append("Content-Security-Policy", "default-src 'none'; frame-ancestors 'none'");

            // Cache control for API responses
            if (context.Request.Path.StartsWithSegments("/mcp"))
            {
                headers.Append("Cache-Control", "no-cache, no-store, must-revalidate");
                headers.Append("Pragma", "no-cache");
                headers.Append("Expires", "0");
            }
        }

        await _next(context);
    }
}

/// <summary>
/// Configuration options for security headers
/// </summary>
public class SecurityHeadersOptions
{
    public bool EnableSecurityHeaders { get; set; } = true;
}

/// <summary>
/// Extension methods for adding security headers middleware
/// </summary>
public static class SecurityHeadersMiddlewareExtensions
{
    public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<SecurityHeadersMiddleware>();
    }

    public static IServiceCollection AddSecurityHeaders(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<SecurityHeadersOptions>(configuration.GetSection("SecurityHeaders"));
        return services;
    }
}