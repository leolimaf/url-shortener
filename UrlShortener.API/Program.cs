
var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .Enrich.WithProperty("Application", "UrlShortener.API")
        .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName));

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
    .MapAuthEndpoints()
    .MapUrlShortenerEndpoints();

app.Run();