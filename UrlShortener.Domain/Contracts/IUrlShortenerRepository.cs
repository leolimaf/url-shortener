using UrlShortener.Domain.Entities;

namespace UrlShortener.Domain.Contracts;

public interface IUrlShortenerRepository
{
    public Task<bool> Exists(string code);
    public Task Add(ShortenedUrl shortenedUrl);
    public Task<ShortenedUrl?> Get(string code);
}