using UrlShortener.Application.DTOs.Requests;
using UrlShortener.Application.DTOs.Response;

namespace UrlShortener.Application.Abstractions;

public interface IUrlShortenerService
{
    Task<ShortenUrlResponse> IncludeShortenedUrl(ShortenUrlRequest request);
    Task<GetOriginalUrlResponse> GetOriginalUrl(string code);
}