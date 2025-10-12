using UrlShortener.Domain.Entities;

namespace UrlShortener.Domain.Contracts;

public interface IUrlShortenerRepository
{
    public Task AddShortenedUrl(ShortenedUrl shortenedUrl);
    public Task AddVisitedUrl(VisitedUrl visitedUrl);
    public Task<string?> GetOriginalUrl(string code);
}