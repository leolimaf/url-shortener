using UrlShortener.API.Contexts;
using UrlShortener.Application.Abstractions;

namespace UrlShortener.API;

public static class DependencyInjection
{
    public static IServiceCollection AddWebApiServices(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddOpenApi();
        
        services.AddHttpContextAccessor();
        services.AddScoped<IUserContext, UserContext>();
        
        return services;
    }

    public static WebApplication AddMiddlewares(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/openapi/v1.json", "v1");
            });
        }

        app.UseHttpsRedirection();
        
        return app;
    }
}