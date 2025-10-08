using UrlShortener.Application.Abstractions;
using UrlShortener.Application.DTOs.Requests;

namespace UrlShortener.API.Endpoints;

public static class UrlShortenerEndpoints
{
    public static IEndpointRouteBuilder MapUrlShortenerEndpoints(this IEndpointRouteBuilder app)
    {
        var appGroup = app.MapGroup("").WithTags("URL Shortener");
        
        appGroup.MapPost("", ShortenUrl)
            .WithName(nameof(ShortenUrl));
        
        appGroup.MapGet("{code}", GetLongUrl)
            .WithName(nameof(GetLongUrl));
        
        return app;
    }

    private static async Task<IResult> ShortenUrl(ShortenUrlRequest request, IUrlShortenerService urlShortenerService, HttpContext httpContext)
    {
        if (!Uri.TryCreate(request.Url, UriKind.Absolute, out _))
            return Results.BadRequest("Invalid URL format.");

        var domain = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";
        
        var shortenedUrl = await urlShortenerService.IncludeShortenedUrl(request, domain);
        return Results.Ok(shortenedUrl);
    }

    private static async Task<IResult> GetLongUrl(string code, IUrlShortenerService urlShortenerService)
    {
        var response = await urlShortenerService.GetLongUrlFromCode(code);

        return string.IsNullOrWhiteSpace(response.LongUrl) 
            ? Results.NotFound() 
            : Results.Redirect(response.LongUrl);
    }
}