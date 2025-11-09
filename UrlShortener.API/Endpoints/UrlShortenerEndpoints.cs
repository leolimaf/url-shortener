using UrlShortener.Application.Abstractions;
using UrlShortener.Application.DTOs.UrlShortener.Requests;

namespace UrlShortener.API.Endpoints;

public static class UrlShortenerEndpoints
{
    public static IEndpointRouteBuilder MapUrlShortenerEndpoints(this IEndpointRouteBuilder app)
    {
        var appGroup = app.MapGroup("url-shortener").WithTags("URL Shortener");
        
        appGroup.MapPost("", IncludeShortenedUrl)
            .WithName(nameof(IncludeShortenedUrl));
        
        appGroup.MapGet("{code}", GetOriginalUrl)
            .WithName(nameof(GetOriginalUrl));
        
        return app;
    }

    private static async Task<IResult> IncludeShortenedUrl(ShortenUrlRequest request, IUrlShortenerService urlShortenerService)
    {
        if (!Uri.TryCreate(request.Url, UriKind.Absolute, out _))
            return Results.BadRequest("Invalid URL format.");

        var response = await urlShortenerService.IncludeShortenedUrl(request);
        return Results.Ok(response);
    }

    private static async Task<IResult> GetOriginalUrl(string code, IUrlShortenerService urlShortenerService)
    {
        var response = await urlShortenerService.GetOriginalUrl(code);

        return string.IsNullOrWhiteSpace(response.OriginalUrl) 
            ? Results.NotFound() 
            : Results.Redirect(response.OriginalUrl);
    }
}