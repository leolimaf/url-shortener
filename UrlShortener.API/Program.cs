
var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddWebApiServices()
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.AddMiddlewares();

if (app.Environment.IsDevelopment())
    app.ApplyMigrations();

app.MapGet("{code}", (string code, HttpContext httpContext) =>
{
    var redirectUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}/api/url-shortener/{code}";
    return Results.Redirect(redirectUrl);
}).ExcludeFromDescription();

app.MapGroup("/api")
    .MapUrlShortenerEndpoints();

app.Run();