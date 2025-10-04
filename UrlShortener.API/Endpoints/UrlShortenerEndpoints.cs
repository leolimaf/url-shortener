namespace UrlShortener.API.Endpoints;

public static class UrlShortenerEndpoints
{
    public static IEndpointRouteBuilder MapUrlShortenerEndpoints(this IEndpointRouteBuilder app)
    {
        var appGroup = app.MapGroup("url-shortener").WithTags("URL Shortener");
        
        appGroup.MapGet("/shorten", Shorten)
            .WithName(nameof(Shorten));
        
        return app;
    }

    private static Task<IResult> Shorten()
    {
        throw new NotImplementedException();
    }
}