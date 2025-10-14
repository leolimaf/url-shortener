using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UrlShortener.Domain.Contracts;
using UrlShortener.Infrastructure.Data;
using UrlShortener.Infrastructure.Repositories;

namespace UrlShortener.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("Default")));
        
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUrlShortenerRepository, UrlShortenerRepository>();

        services.AddStackExchangeRedisCache(options => options.Configuration = configuration.GetConnectionString("Redis"));
        services.AddHybridCache(options =>
        {
            options.MaximumPayloadBytes = 1024 * 1024;
            options.MaximumKeyLength = 1024;
            options.DefaultEntryOptions = new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromMinutes(10),
                LocalCacheExpiration = TimeSpan.FromMinutes(10)
            };
        });
        
        return services;
    }
}