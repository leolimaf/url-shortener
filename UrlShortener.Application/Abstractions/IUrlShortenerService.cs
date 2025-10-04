namespace UrlShortener.Application.Abstractions;

public interface IUrlShortenerService
{
    Task<string> GenerateUniqueCode();
}