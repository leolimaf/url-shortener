using UrlShortener.Application.DTOs.Requests;

namespace UrlShortener.Application.Abstractions;

public interface IUrlShortenerService
{
    Task<string> GenerateUniqueCode();
    Task<string> IncludeShortenedUrl(ShortenUrlRequest request, string code);
}