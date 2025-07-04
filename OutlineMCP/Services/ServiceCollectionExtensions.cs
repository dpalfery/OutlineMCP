using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OutlineMCP.Settings;

namespace OutlineMCP.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOutlineServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register settings
        services.Configure<OutlineSettings>(configuration.GetSection("Outline"));
        services.AddSingleton<OutlineSettings>();
        
        // Register HttpClient for OutlineTools with security configurations
        services.AddHttpClient<OutlineTools>(client =>
        {
            // Set reasonable timeouts
            client.Timeout = TimeSpan.FromSeconds(30);
            
            // Add User-Agent header
            client.DefaultRequestHeaders.Add("User-Agent", "OutlineMCP/1.0");
            
            // Disable default caching
            client.DefaultRequestHeaders.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue
            {
                NoCache = true,
                NoStore = true
            };
        })
        .ConfigurePrimaryHttpMessageHandler(() =>
        {
            var handler = new HttpClientHandler();
            
            // Security configurations
            handler.CheckCertificateRevocationList = true;
            handler.SslProtocols = System.Security.Authentication.SslProtocols.Tls12 | System.Security.Authentication.SslProtocols.Tls13;
            
            return handler;
        });
        
        return services;
    }
}