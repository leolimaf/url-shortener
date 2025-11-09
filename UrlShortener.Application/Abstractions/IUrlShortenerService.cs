using UrlShortener.Application.DTOs.UrlShortener.Requests;
using UrlShortener.Application.DTOs.UrlShortener.Responses;

namespace UrlShortener.Application.Abstractions;

public interface IUrlShortenerService
{
    Task<ShortenUrlResponse> IncludeShortenedUrl(ShortenUrlRequest request);
    Task<GetOriginalUrlResponse> GetOriginalUrl(string code);
}