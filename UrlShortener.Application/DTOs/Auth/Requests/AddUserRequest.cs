namespace UrlShortener.Application.DTOs.Auth.Requests;

public record AddUserRequest(
    string FirstName,
    string LastName,
    DateOnly? BirthDate,
    string Phone,
    string Email,
    string Password
);