using UrlShortener.Domain.Entities;

namespace UrlShortener.Domain.Repositories;

public interface IUrlShortenerRepository
{
    public Task<bool> Exists(string code);
    public Task Add(ShortenedUrl shortenedUrl);
}