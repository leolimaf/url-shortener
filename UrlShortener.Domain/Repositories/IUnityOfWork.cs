namespace UrlShortener.Domain.Repositories;

public interface IUnitOfWork : IDisposable
{
    Task<int> SaveAsync();
}