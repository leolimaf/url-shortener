namespace UrlShortener.Application.DTOs.Auth.Responses;

public record GetUserResponse(long Id, string FullName, string Email);