namespace UrlShortener.Application.DTOs.Auth.Requests;

public record GenarateTokensRequest(string Email, string Password);