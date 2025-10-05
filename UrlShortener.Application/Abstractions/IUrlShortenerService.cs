using UrlShortener.Application.DTOs.Requests;
using UrlShortener.Application.DTOs.Response;

namespace UrlShortener.Application.Abstractions;

public interface IUrlShortenerService
{
    Task<string> GenerateUniqueCode();
    Task<ShortenUrlResponse> IncludeShortenedUrl(ShortenUrlRequest request, string domain, string code);
    Task<GetLongUrlResponse> GetLongUrlFromCode(string code);
}