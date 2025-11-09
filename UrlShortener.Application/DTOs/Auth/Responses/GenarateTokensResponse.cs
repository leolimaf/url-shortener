namespace UrlShortener.Application.DTOs.Auth.Responses;

public record GenarateTokensResponse(string AccessToken, string RefreshToken);