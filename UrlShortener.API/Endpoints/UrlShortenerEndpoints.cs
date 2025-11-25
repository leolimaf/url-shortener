using UrlShortener.Application.Abstractions;
using UrlShortener.Application.DTOs.UrlShortener.Requests;
using UrlShortener.Application.DTOs.UrlShortener.Responses;

namespace UrlShortener.API.Endpoints;

public static class UrlShortenerEndpoints
{
    public static IEndpointRouteBuilder MapUrlShortenerEndpoints(this IEndpointRouteBuilder app)
    {
        var appGroup = app.MapGroup("url-shortener")
            .WithTags("URL Shortener")
            .AllowAnonymous();
        
        appGroup.MapPost("", IncludeShortenedUrl)
            .WithName(nameof(IncludeShortenedUrl))
            .Produces<ShortenUrlResponse>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
        
        appGroup.MapGet("{code}", GetOriginalUrl)
            .WithName(nameof(GetOriginalUrl))
            .Produces(StatusCodes.Status302Found)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
        
        return app;
    }

    private static async Task<IResult> IncludeShortenedUrl(ShortenUrlRequest request, IUrlShortenerService urlShortenerService)
    {
        if (!Uri.TryCreate(request.Url, UriKind.Absolute, out _))
            return Results.BadRequest(new { errorMessage = "Invalid URL format." });

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