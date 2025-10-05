using UrlShortener.Application.Abstractions;
using UrlShortener.Application.DTOs.Requests;

namespace UrlShortener.API.Endpoints;

public static class UrlShortenerEndpoints
{
    public static IEndpointRouteBuilder MapUrlShortenerEndpoints(this IEndpointRouteBuilder app)
    {
        var appGroup = app.MapGroup("url-shortener").WithTags("URL Shortener");
        
        appGroup.MapPost("/shorten", ShortenUrl)
            .WithName(nameof(ShortenUrl));
        
        return app;
    }

    private static async Task<IResult> ShortenUrl(ShortenUrlRequest request, IUrlShortenerService urlShortenerService, HttpContext httpContext)
    {
        if (!Uri.TryCreate(request.Url, UriKind.Absolute, out _))
            return Results.BadRequest("Invalid URL format.");

        var domain = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";
        var code = await urlShortenerService.GenerateUniqueCode();
        
        var shortenedUrl = await urlShortenerService.IncludeShortenedUrl(request, domain, code);
        return Results.Ok(shortenedUrl);
    }
}