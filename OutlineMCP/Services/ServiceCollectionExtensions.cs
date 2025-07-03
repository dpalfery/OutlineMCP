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
        
        // Register HttpClient for OutlineTools
        services.AddHttpClient<OutlineTools>();
        
        return services;
    }
}