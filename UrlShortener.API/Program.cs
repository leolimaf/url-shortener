
var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddWebApiServices()
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.AddMiddlewares();

app.MapGroup("/api")
    .MapUrlShortenerEndpoints();

app.Run();