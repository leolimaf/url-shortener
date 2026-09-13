using Microsoft.EntityFrameworkCore;
using UrlShortener.API.Contexts;
using UrlShortener.Application.Abstractions;
using UrlShortener.Infrastructure.Data;

namespace UrlShortener.API;

public static class DependencyInjection
{
    public static IServiceCollection AddWebApiServices(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, _, _) =>
            {
                document.Info = new Microsoft.OpenApi.OpenApiInfo
                {
                    Title = "URL Shortener API",
                    Version = "v1",
                    Description = "API para encurtar URLs"
                };

                // Definição do esquema de segurança Bearer
                var securityScheme = new Microsoft.OpenApi.OpenApiSecurityScheme
                {
                    Type = Microsoft.OpenApi.SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.ParameterLocation.Header,
                    Description = "Insira apenas o Bearer token (JWT)"
                };

                document.Components ??= new Microsoft.OpenApi.OpenApiComponents();
                document.Components.SecuritySchemes = new Dictionary<string, Microsoft.OpenApi.IOpenApiSecurityScheme>
                {
                    ["Bearer"] = securityScheme
                };

                // Adiciona requisito global
                document.Security =
                [
                    new Microsoft.OpenApi.OpenApiSecurityRequirement
                    {
                        [new Microsoft.OpenApi.OpenApiSecuritySchemeReference("Bearer", document)] = []
                    }
                ];

                return Task.CompletedTask;
            });
        });
        
        services.AddHttpContextAccessor();
        services.AddScoped<IUserContext, UserContext>();
        
        return services;
    }

    public static WebApplication AddMiddlewares(this WebApplication app)
    {
        app.UseSerilogRequestLogging();
        
        app.UseAuthentication();
        app.UseAuthorization();
        
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
    
    public static void ApplyMigrations(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.Migrate();
    }
}