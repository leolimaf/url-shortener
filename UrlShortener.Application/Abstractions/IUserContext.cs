namespace UrlShortener.Application.Abstractions;

public interface IUserContext
{
    string? UserAgent { get; }
    string? Referer { get; }
    string? IpAddress { get; }
}