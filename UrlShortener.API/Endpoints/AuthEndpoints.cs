using System.Text.RegularExpressions;
using UrlShortener.Application.Abstractions;
using UrlShortener.Application.DTOs.Auth.Requests;
using UrlShortener.Application.DTOs.Auth.Responses;

namespace UrlShortener.API.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var appGroup = app.MapGroup("/auth").WithTags("Auth");
        
        appGroup.MapPost("/user", RegisterUser)
            .WithName(nameof(RegisterUser))
            .Produces<GetUserResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
        
        appGroup.MapGet("/user/{id:long}", GetUserById)
            .WithName(nameof(GetUserById))
            .RequireAuthorization(x => x.RequireRole("admin"))
            .Produces<GetUserResponse>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status500InternalServerError);
        
        appGroup.MapPost("/access-token", GenarateTokens)
            .WithName(nameof(GenarateTokens))
            .Produces<GenarateTokensResponse>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
        
        appGroup.MapPost("/refresh-token", RefreshTokens)
            .WithName(nameof(RefreshTokens))
            .Produces<RefreshTokensResponse>()
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
        
        return app;
    }

    private static async Task<IResult> RegisterUser(IAuthService authService, AddUserRequest userDto)
    {
        if (string.IsNullOrWhiteSpace(userDto.FirstName) 
            || string.IsNullOrWhiteSpace(userDto.LastName) 
            || string.IsNullOrWhiteSpace(userDto.Email) 
            || string.IsNullOrWhiteSpace(userDto.Password))
        {
            return TypedResults.BadRequest(new { errorMessage = "Fill in all required fields." });
        }
        
        Regex emailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
        if (!emailRegex.IsMatch(userDto.Email))
            return Results.BadRequest(new { errorMessage = "Invalid Email." });

        var userId = await authService.RegisterUser(userDto);
        
        if (userId == 0)
            return TypedResults.Conflict(new { errorMessage = "User with this email already exists." });
        
        var createdUser = await authService.GetUserById(userId);
        
        return TypedResults.CreatedAtRoute(createdUser, nameof(GetUserById), new { id = userId });
    }
    
    private static async Task<IResult> GetUserById(IAuthService authService, long id)
    {
        var userDto = await authService.GetUserById(id);
        
        return userDto is not null
            ? TypedResults.Ok(userDto)
            : TypedResults.NotFound();
    }
    
    private static async Task<IResult> GenarateTokens(IAuthService authService, GenarateTokensRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return TypedResults.BadRequest(new { errorMessage = "All fields are required." });
        
        Regex emailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
        if (!emailRegex.IsMatch(request.Email))
            return Results.BadRequest(new { errorMessage = "Invalid Email." });
        
        var response = await authService.GenarateTokens(request);
        
        return response is not null
            ? TypedResults.Ok(response)
            : TypedResults.BadRequest(new { errorMessage = "Invalid credentials." });
    }
    
    private static async Task<IResult> RefreshTokens(IAuthService authService, RefreshTokensRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            return TypedResults.BadRequest(new { errorMessage = "The refresh token is required." });
        
        var response = await authService.RefreshTokens(request);

        return response is not null
            ? TypedResults.Ok(response)
            : TypedResults.Unauthorized();
    }
}