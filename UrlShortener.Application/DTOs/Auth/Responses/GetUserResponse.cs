namespace UrlShortener.Application.DTOs.Auth.Responses;

public record GetUserResponse(
    long Id,
    string FullName,
    DateOnly? BirthDate,
    string? Phone,
    string Email,
    bool IsEmailConfirmed
);