namespace UrlShortener.Application.DTOs.Auth.Responses;

public record RefreshTokensResponse(string AccessToken, string RefreshToken);