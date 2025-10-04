namespace UrlShortener.Domain.Repositories;

public interface IUrlShortenerRepository
{
    public Task<bool> Exists(string code);
}